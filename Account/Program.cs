using AccountServices.Features;
using AccountServices.Features.Accounts;
using AccountServices.Features.Accounts.CreateAccount;
using AccountServices.Features.Accounts.Services;
using AccountServices.Features.Accounts.UpdateAccount;
using AccountServices.Features.Events;
using AccountServices.Features.Examples;
using AccountServices.Features.Transactions.RegisterTransaction;
using AccountServices.Features.Transactions.Services;
using AccountServices.Features.Transactions.TransferTransaction;
using FluentValidation;
using Hangfire;
using Hangfire.PostgreSql;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Swashbuckle.AspNetCore.Filters;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using HealthChecks.UI.Client;

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
    })
    .AddApplicationPart(typeof(EventsController).Assembly); 

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Accounts API",
        Version = "v1",
        Description = "API для работы с аккаунтами"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Введите токен в формате: Bearer {your JWT token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

    options.SwaggerDoc("events", new OpenApiInfo
    {
        Title = "События",
        Version = "v1",
        Description = "Документация по событиям системы"
    });

    options.ExampleFilters();
});

builder.Services.AddSwaggerExamplesFromAssemblyOf<EventsController>();

builder.Services.AddSwaggerExamplesFromAssemblyOf<AccountOpenedExample>();
builder.Services.AddSwaggerExamplesFromAssemblyOf<MoneyCreditedExample>();
builder.Services.AddSwaggerExamplesFromAssemblyOf<MoneyDebitedExample>();
builder.Services.AddSwaggerExamplesFromAssemblyOf<TransferCompletedExample>();

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
{
#pragma warning disable CS0618 // Type or member is obsolete
    configuration.UsePostgreSqlStorage(builder.Configuration.GetConnectionString("DefaultConnection"));
#pragma warning restore CS0618 // Type or member is obsolete
});

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

builder.Services.AddHealthChecks()
    .AddRabbitMQ(
        "amqp://user:password@localhost:5672/",
        name: "rabbitmq",
        timeout: TimeSpan.FromSeconds(3),
        tags: ["ready"]);

var app = builder.Build();

app.UseHttpLogging(); 
app.UseSerilogRequestLogging(options =>
{
    options.MessageTemplate =
        "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms CorrelationId={CorrelationId}";
});

app.UseHangfireDashboard();

app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = _ => false, 
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready"), 
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});


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


// ReSharper disable once RedundantTypeDeclarationBody Для тестов
public partial class Program { }