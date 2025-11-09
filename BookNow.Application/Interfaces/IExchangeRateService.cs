using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookNow.Application.Interfaces
{
    public interface IExchangeRateService
    {
        
        public const string CurrencyRatesHashKey = "exchange:rates:current";
        public const string BaseCurrency = "EUR";

 
        Task<Dictionary<string, decimal>> GetRatesAsync();

        decimal Convert(decimal amount, string sourceCurrency, string targetCurrency, Dictionary<string, decimal> rates);

        Task SetRatesAsync(Dictionary<string, decimal> rates);
    }
}
