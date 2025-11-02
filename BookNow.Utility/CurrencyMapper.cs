using System.Collections.Generic;
using System.Linq;

namespace BookNow.Utility
{
    public static class CurrencyMapper
    {
       
        private static readonly Dictionary<string, string> CountryCodeToSymbolMap = new Dictionary<string, string>
        {
            { "IND", "₹" },
            { "USA", "$" },
            { "GBR", "£" },
            { "AUS", "$" },
            { "JPN", "¥" }
        };

        public static string GetSymbolByCountryCode(string countryCode)
        {
            if (string.IsNullOrWhiteSpace(countryCode))
            {
                return "₹"; 
            }

            if (CountryCodeToSymbolMap.TryGetValue(countryCode.ToUpperInvariant(), out string symbol))
            {
                return symbol;
            }

            return "₹"; 
        }
    }
}
