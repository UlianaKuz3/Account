namespace AccountServices.Features.Accounts.Services
{
    public interface ICurrencyService
    {
        bool IsSupported(string currency);
    }
}
