using AccountServices.Features;
using AccountServices.Features.Accounts;
using AccountServices.Features.Accounts.CreateAccount;
using AccountServices.Features.Accounts.Services;
using AccountServices.Features.Accounts.UpdateAccount;
using AccountServices.Features.Transactions.RegisterTransaction;
using AccountServices.Features.Transactions.Services;
using AccountServices.Features.Transactions.TransferTransaction;
using FluentValidation;
using Hangfire;
using Hangfire.PostgreSql;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RabbitMQ.Client;
using Serilog;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IClientVerificationService, ClientVerificationServiceStub>();
builder.Services.AddSingleton<ICurrencyService, CurrencyServiceStub>();

builder.Services.AddScoped<IValidator<CreateAccountCommand>, CreateAccountCommandValidator>();
builder.Services.AddScoped<IValidator<UpdateAccountCommand>, UpdateAccountCommandValidator>();
builder.Services.AddScoped<IValidator<RegisterTransactionCommand>, RegisterTransactionCommandValidator>();
builder.Services.AddScoped<IValidator<TransferTransactionCommand>, TransferTransactionCommandValidator>();


builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Accounts API",
        Version = "v1",
        Description = "API дл€ работы с аккаунтами"
    });

    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "¬ведите токен в формате: Bearer {your JWT token}"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var jwtConfig = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtConfig["Key"]!);

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtConfig["Issuer"],
            ValidAudience = jwtConfig["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IAccountRepository, AccountRepository>();

builder.Services.AddHangfire(configuration =>
    configuration.UsePostgreSqlStorage(options =>
        options.UseNpgsqlConnection(builder.Configuration.GetConnectionString("DefaultConnection"))));

builder.Services.AddHangfireServer();

builder.Services.AddScoped<InterestAccrualService>();

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console(outputTemplate:
        "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddHttpLogging(o =>
{
    o.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.All;
});

builder.Services.AddHttpClient("default")
    .AddHttpMessageHandler<LoggingHttpHandler>();

var app = builder.Build();

app.UseHttpLogging(); 
app.UseSerilogRequestLogging(options =>
{
    options.MessageTemplate =
        "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms CorrelationId={CorrelationId}";
});

app.UseHangfireDashboard();

RecurringJob.AddOrUpdate<OutboxPublisherJob>(
    job => job.PublishOutboxMessages(),
    Cron.Minutely);

RecurringJob.AddOrUpdate<InterestAccrualService>(
    "InterestAccrualJob",
    service => service.AccrueInterestAsync(),
    Cron.Daily);

RecurringJob.AddOrUpdate("setup-rabbit-topology",
    () => RabbitTopologyInitializer.InitializeTopology("rabbitmq"),
    Cron.Daily);

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseCors("AllowAll");

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.MapControllers();

app.Run();


// ReSharper disable once RedundantTypeDeclarationBody ƒл€ тестов
public partial class Program { }