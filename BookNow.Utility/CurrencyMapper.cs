using Microsoft.Extensions.Logging;
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
        private static readonly Dictionary<string, string> CurrencyToSymbolMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        { "INR", "₹" },
        { "USD", "$" },
        { "GBP", "£" },
        { "AUD", "$" },
        { "JPY", "¥" },
        { "EUR", "€" },
        { "CAD", "C$" }
       
    };
        private static readonly Dictionary<string, string> CountryToCurrencyMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            // IMPORTANT: Adjust this map if your 'Country Code' in the DB refers to a different currency (e.g., JPN could map to something else)
            { "IND", "INR" }, // India -> Indian Rupee
            { "JPN", "JPY" }, // Japan -> Japanese Yen
            { "USA", "USD" }, // United States -> US Dollar
            { "GBR", "GBP" }, // Great Britain -> Pound Sterling
            { "AUS", "AUD" }, // Australia -> Australian Dollar
            { "CAD", "CAD" }, // Canada -> Canadian Dollar
            { "EUR", "EUR" }  // If you use EUR for Eurozone countries in your DB, map it.
            // Add other mappings as needed
        };
        public static string GetCurrencyCode(string countryCode)
        {
            if (string.IsNullOrWhiteSpace(countryCode))
            {
                return "USD"; // Safe default for API calls
            }

            if (CountryToCurrencyMap.TryGetValue(countryCode, out string currencyCode))
            {
                return currencyCode;
            }
           
           
            return "USD"; // Default to Base Currency if mapping fails
        }
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
        public static string GetSymbolByCurrencyCode(string currencyCode)
        {
            if (string.IsNullOrWhiteSpace(currencyCode))
                return "₹"; // default

            if (CurrencyToSymbolMap.TryGetValue(currencyCode.ToUpperInvariant(), out string symbol))
                return symbol;

            return currencyCode; // fallback: display currency code if unknown
        }
    }
}
