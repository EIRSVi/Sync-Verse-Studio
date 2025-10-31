using System;

namespace SyncVerseStudio.Services
{
    /// <summary>
    /// Handles dual-currency operations for KHR and USD
    /// Exchange Rate: 1 USD = 4000 KHR (configurable)
    /// </summary>
    public class CurrencyService
    {
        private const decimal EXCHANGE_RATE = 4000m; // 1 USD = 4000 KHR
        private const decimal USD_THRESHOLD = 1000m; // Amounts > 1000 assumed to be KHR

        public enum Currency
        {
            USD,
            KHR
        }

        /// <summary>
        /// Convert amount from one currency to another
        /// </summary>
        public static decimal Convert(decimal amount, Currency from, Currency to)
        {
            if (from == to) return amount;

            if (from == Currency.KHR && to == Currency.USD)
            {
                return amount / EXCHANGE_RATE;
            }
            else if (from == Currency.USD && to == Currency.KHR)
            {
                return amount * EXCHANGE_RATE;
            }

            return amount;
        }

        /// <summary>
        /// Auto-detect currency based on amount
        /// </summary>
        public static Currency DetectCurrency(decimal amount)
        {
            return amount > USD_THRESHOLD ? Currency.KHR : Currency.USD;
        }

        /// <summary>
        /// Format amount for display with currency symbol
        /// </summary>
        public static string Format(decimal amount, Currency currency)
        {
            return currency == Currency.USD 
                ? $"${amount:N2}" 
                : $"{amount:N0} ៛"; // Riel symbol
        }

        /// <summary>
        /// Format amount with auto-detection
        /// </summary>
        public static string FormatAuto(decimal amount)
        {
            var currency = DetectCurrency(amount);
            if (currency == Currency.KHR)
            {
                // Convert to USD for display
                amount = Convert(amount, Currency.KHR, Currency.USD);
            }
            return Format(amount, Currency.USD);
        }

        /// <summary>
        /// Calculate change in appropriate currency
        /// </summary>
        public static (decimal changeAmount, Currency changeCurrency) CalculateChange(
            decimal totalAmount, 
            decimal paidAmount, 
            Currency paidCurrency,
            Currency preferredChangeCurrency = Currency.USD)
        {
            // Convert total to paid currency for comparison
            var totalInPaidCurrency = Convert(totalAmount, DetectCurrency(totalAmount), paidCurrency);
            
            var change = paidAmount - totalInPaidCurrency;
            
            if (change < 0)
            {
                throw new InvalidOperationException("Insufficient payment");
            }

            // Convert change to preferred currency
            var changeInPreferred = Convert(change, paidCurrency, preferredChangeCurrency);
            
            return (changeInPreferred, preferredChangeCurrency);
        }

        /// <summary>
        /// Format dual currency display (USD / KHR)
        /// </summary>
        public static string FormatDual(decimal amount)
        {
            var currency = DetectCurrency(amount);
            decimal usdAmount, khrAmount;

            if (currency == Currency.USD)
            {
                usdAmount = amount;
                khrAmount = Convert(amount, Currency.USD, Currency.KHR);
            }
            else
            {
                khrAmount = amount;
                usdAmount = Convert(amount, Currency.KHR, Currency.USD);
            }

            return $"${usdAmount:N2} / {khrAmount:N0}៛";
        }

        /// <summary>
        /// Parse currency string to decimal
        /// </summary>
        public static (decimal amount, Currency currency) Parse(string input)
        {
            input = input.Trim();
            
            if (input.StartsWith("$"))
            {
                var amount = decimal.Parse(input.TrimStart('$').Replace(",", ""));
                return (amount, Currency.USD);
            }
            else if (input.Contains("៛") || input.ToUpper().Contains("KHR"))
            {
                var amount = decimal.Parse(input.Replace("៛", "").Replace("KHR", "").Replace(",", "").Trim());
                return (amount, Currency.KHR);
            }
            else
            {
                var amount = decimal.Parse(input.Replace(",", ""));
                return (amount, DetectCurrency(amount));
            }
        }

        /// <summary>
        /// Get exchange rate
        /// </summary>
        public static decimal GetExchangeRate()
        {
            return EXCHANGE_RATE;
        }

        /// <summary>
        /// Validate payment amount
        /// </summary>
        public static bool ValidatePayment(decimal totalAmount, decimal paidAmount, Currency paidCurrency)
        {
            var totalInPaidCurrency = Convert(totalAmount, DetectCurrency(totalAmount), paidCurrency);
            return paidAmount >= totalInPaidCurrency;
        }
    }
}
