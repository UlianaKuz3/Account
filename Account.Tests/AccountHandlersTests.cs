using AccountServices.Features;
using AccountServices.Features.Accounts;
using AccountServices.Features.Accounts.CreateAccount;
using AccountServices.Features.Accounts.GetAccountById;
using AccountServices.Features.Accounts.GetAllAccounts;
using AccountServices.Features.Accounts.Services;
using Moq;


namespace AccountServices.Tests
{
    public class AccountHandlersTests
    {
        private readonly Mock<IAccountRepository> _repositoryMock = new();
        private readonly Mock<IClientVerificationService> _clientServiceMock = new();
        private readonly Mock<ICurrencyService> _currencyServiceMock = new();

        [Fact]
        public async Task CreateAccountHandler_ShouldCreateAccount_WhenValidData()
        {
            var ownerId = Guid.NewGuid();
            _clientServiceMock.Setup(s => s.Exists(ownerId)).Returns(true);
            _currencyServiceMock.Setup(s => s.IsSupported("USD")).Returns(true);

            var repository = _repositoryMock.Object;
            var handler = new CreateAccountHandler(repository, _clientServiceMock.Object, _currencyServiceMock.Object);

            var command = new CreateAccountCommand(
                ownerId,
                AccountType.Checking,
                "USD",
                1000m,
                0.01m
            );

            var result = await handler.Handle(command, CancellationToken.None);

            _repositoryMock.Verify(r => r.Add(It.Is<Account>(a =>
                a.OwnerId == ownerId &&
                a.Currency == "USD" &&
                a.Balance == 1000m)), Times.Once);

            Assert.Equal(ownerId, result.OwnerId);
            Assert.Equal("USD", result.Currency);
            Assert.Equal(1000m, result.Balance);
        }

        [Fact]
        public async Task GetAccountByIdHandler_ShouldReturnAccount_WhenExists()
        {
            var accountId = Guid.NewGuid();
            var account = new Account { Id = accountId };
            _repositoryMock.Setup(r => r.GetById(accountId)).Returns(account);

            var handler = new GetAccountByIdHandler(_repositoryMock.Object);
            var query = new GetAccountByIdQuery(accountId);

            var result = await handler.Handle(query, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(accountId, result.Id);
        }

        [Fact]
        public async Task GetAccountByIdHandler_ShouldThrow_WhenNotFound()
        {
            var accountId = Guid.NewGuid();
            _repositoryMock.Setup(r => r.GetById(accountId)).Returns((Account?)null);

            var handler = new GetAccountByIdHandler(_repositoryMock.Object);
            var query = new GetAccountByIdQuery(accountId);

            await Assert.ThrowsAsync<NotFoundException>(() =>
                handler.Handle(query, CancellationToken.None));
        }

        [Fact]
        public async Task GetAllAccountsHandler_ShouldReturnAllAccounts()
        {
            var accounts = new List<Account>
            {
                new (){ Id = Guid.NewGuid() },
                new () { Id = Guid.NewGuid() }
            };
            _repositoryMock.Setup(r => r.GetAll()).Returns(accounts);

            var handler = new GetAllAccountsHandler(_repositoryMock.Object);
            var query = new GetAllAccountsQuery();

            var result = await handler.Handle(query, CancellationToken.None);

            Assert.Equal(accounts.Count, result.Count());
        }
    }

}
