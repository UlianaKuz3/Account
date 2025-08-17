using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Testcontainers.PostgreSql;

namespace AccountServices.Tests
{
    public class AccountIntegrationTests : IAsyncLifetime
    {
        private readonly PostgreSqlContainer _postgres;
        private readonly WebApplicationFactory<Program> _factory;
        private HttpClient _client;

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
            await _postgres.StartAsync();
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
    }
}
