namespace Account.Features.Accounts.Services
{
    public class CurrencyServiceStub : ICurrencyService
    {
        private static readonly HashSet<string> SupportedCurrencies =
        [
            "USD", "EUR", "RUB"
        ];

        public bool IsSupported(string currency)
        {
            return SupportedCurrencies.Contains(currency.ToUpperInvariant());
        }
    }
}
