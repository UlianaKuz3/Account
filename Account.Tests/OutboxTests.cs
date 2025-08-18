using AccountServices.Features;
using AccountServices.Features.Accounts;
using AccountServices.Features.Entities;
using AccountServices.Features.Transactions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Testcontainers.PostgreSql;

namespace AccountServices.Tests
{
    public class OutboxTests : IClassFixture<WebApplicationFactory<Program>>, IAsyncLifetime
    {
        private HttpClient _client;
        private readonly AppDbContext _db;
        private readonly WebApplicationFactory<Program> _factory;
        private readonly IServiceProvider _provider;
        private readonly ILogger<OutboxPublisherJob> _logger;
        private readonly IConnection _rabbitConnection;
        private IServiceScope _scope;
        private readonly PostgreSqlContainer _postgres;


        public OutboxTests()
        {
            _postgres = new PostgreSqlBuilder()
                // ReSharper disable once StringLiteralTypo Намеренное написание
                .WithDatabase("testdb")
                .WithUsername("postgres")
                .WithPassword("postgres")
                .Build();

            _factory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureAppConfiguration((_, _) =>
                    {
                        Environment.SetEnvironmentVariable("ConnectionStrings__DefaultConnection",
                            _postgres.GetConnectionString());
                    });
                });
            _client = _factory.CreateClient();
            _db = _factory.Services.GetRequiredService<AppDbContext>();
        }

        public async Task InitializeAsync()
        {
            await _postgres.StartAsync();
            _client = _factory.CreateClient();
            _scope = _factory.Services.CreateScope();
        }

        public async Task DisposeAsync()
        {
            await _postgres.DisposeAsync();
            _scope?.Dispose();
            _client?.Dispose();
        }



        
    }
}
