namespace Account.Features.Accounts.Services
{
    public interface ICurrencyService
    {
        bool IsSupported(string currency);
    }
}
