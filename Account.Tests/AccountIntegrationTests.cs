using AccountServices.Features;
using AccountServices.Features.Accounts;
using AccountServices.Features.Entities;
using AccountServices.Features.Transactions;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;

namespace AccountServices.Tests
{
    public class AccountIntegrationTests : IAsyncLifetime
    {
        private readonly PostgreSqlContainer _postgres;
        private WebApplicationFactory<Program> _factory;
        private HttpClient _client;
        private readonly RabbitMqContainer _rabbitMqContainer;

        private Guid _account1Id;
        private Guid _account2Id;

        public AccountIntegrationTests()
        {
            _client = new HttpClient();

            _postgres = new PostgreSqlBuilder()
                // ReSharper disable once StringLiteralTypo Намеренное написание
                .WithDatabase("testdb")
                .WithUsername("postgres")
                .WithPassword("postgres")
                .Build();

           // _postgres.StartAsync().res
            //_postgres.GetConnectionString();

            _rabbitMqContainer = new RabbitMqBuilder()
                .WithImage("rabbitmq:3.13-management")
                .WithPortBinding(5672, true) // Expose AMQP port
                .WithPortBinding(15672, true) 
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
        }

        public async Task InitializeAsync()
        {
            await Task.WhenAll(
                _postgres.StartAsync(),
                _rabbitMqContainer.StartAsync()
            );

            _factory = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.RemoveAll<DbContextOptions<AppDbContext>>();
                    services.AddDbContext<AppDbContext>(options =>
                        options.UseNpgsql(_postgres.GetConnectionString()));

   
                    services.AddSingleton(_ =>
                    {
                        var factory = new ConnectionFactory
                        {
                            HostName = "localhost",
                            Port = _rabbitMqContainer.GetMappedPublicPort(5672),
                            UserName = "rabbitmq",
                            Password = "rabbitmq"
                        };

                        return factory.CreateConnection();
                    });


                });
            });

            _client = _factory.CreateClient();
            

            // ReSharper disable once StringLiteralTypo Намеренное написание
            var login = new { Username = "testuser" };
            var loginResponse = await _client.PostAsJsonAsync("/api/Auth/login", login);
            loginResponse.EnsureSuccessStatusCode();

            var token = await loginResponse.Content.ReadAsStringAsync();

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            _account1Id = await CreateAccount(1000m);
            _account2Id = await CreateAccount(1000m);
        }

        public async Task DisposeAsync()
        {
            await _postgres.StopAsync();
            _client.Dispose();
        }

        private async Task<Guid> CreateAccount(decimal initialBalance)
        {
            var command = new
            {
                OwnerId = Guid.NewGuid(),
                Type = "Checking",
                Currency = "USD",
                Balance = initialBalance,
                InterestRate = 0.01m
            };

            var response = await _client.PostAsJsonAsync("/api/accounts", command);
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var account = await response.Content.ReadFromJsonAsync<AccountDto>();
            return account?.Id ?? throw new InvalidOperationException("Failed to create account: response was null");
        }

        private async Task<decimal> GetBalance(Guid accountId)
        {
            var response = await _client.GetAsync($"/api/accounts/{accountId}");
            response.EnsureSuccessStatusCode();

            var account = await response.Content.ReadFromJsonAsync<AccountDto>();
            return account?.Balance ?? throw new InvalidOperationException("Account not found or balance is null");
        }

        [Fact]
        public async Task Parallel_Transfers_Should_Keep_Total_Balance()
        {
            decimal initialTotal = await GetBalance(_account1Id) + await GetBalance(_account2Id);

            var tasks = Enumerable.Range(0, 5).Select(async _ =>
            {
                var dto = new
                {
                    FromAccountId = _account1Id,
                    ToAccountId = _account2Id,
                    Amount = 10m,
                    Currency = "USD"
                };

                var response = await _client.PostAsJsonAsync("/api/transactions/transfer", dto);
                if (response.StatusCode != HttpStatusCode.Conflict)
                    response.EnsureSuccessStatusCode();
            });

            await Task.WhenAll(tasks);

            decimal finalTotal = await GetBalance(_account1Id) + await GetBalance(_account2Id);

            finalTotal.Should().Be(initialTotal);
        }

        public record AccountDto(Guid Id, decimal Balance);

        [Fact]
        public async Task OutboxPublishesAfterFailure()
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var accountId = Guid.NewGuid();
            db.Outbox.Add(new OutboxMessage
            {
                Id = Guid.NewGuid(),
                Type = "account.opened",
                Payload = JsonSerializer.Serialize(new { AccountId = accountId }),
                RoutingKey = "account.opened",
                CreatedAt = DateTime.UtcNow
            });
            await db.SaveChangesAsync();

            var job = new OutboxPublisherJob(
                scope.ServiceProvider.GetRequiredService<IServiceScopeFactory>(),
                scope.ServiceProvider.GetRequiredService<ILogger<OutboxPublisherJob>>(),
                scope.ServiceProvider.GetRequiredService<IConnection>());

            await job.PublishOutboxMessages();

            using var assertScope = _factory.Services.CreateScope();
            var assertDb = assertScope.ServiceProvider.GetRequiredService<AppDbContext>();

            var msg = await assertDb.Outbox.FirstAsync(m => m.Type == "account.opened");
            Assert.NotNull(msg.Processed);
        }

        [Fact]
        public async Task ClientBlockedPreventsDebit()
        {
            var clientId = Guid.NewGuid();
            var account = new Account { Id = Guid.NewGuid(), OwnerId = clientId, Balance = 1000, Currency = "USD", IsBlocked = true };

            var res = await _client.PostAsJsonAsync("/api/accounts", account);
            res.StatusCode.Should().Be(HttpStatusCode.Created);
            var createdAccount = await res.Content.ReadFromJsonAsync<AccountDto>();
            createdAccount.Should().NotBeNull("Account creation should return the created account");

            var dto = new RegisterTransactionDto
            {
                AccountId = createdAccount.Id,
                CounterpartyAccountId = Guid.NewGuid(),
                Amount = 100,
                Currency = "USD",
                Type = TransactionType.Debit,
                Description = "Test debit"
            };

            var response = await _client.PostAsJsonAsync("api/Transactions", dto);

            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);

        }
    }
}
