using System;
using System.Collections.Generic;

namespace Core.Economy
{
    /// <summary>
    /// Container that holds different currencies.
    /// </summary>
    public class Wallet
    {
        private readonly Dictionary<CurrencyData, int> _currencies = new Dictionary<CurrencyData, int>();

        /// <summary>
        /// called whenever a currency changes.
        /// </summary>
        public event Action<CurrencyData, int> OnCurrencyChanged;
        
        /// <summary>
        /// gets the amount of a specific currency.
        /// </summary>
        /// <param name="currency">currency to check</param>
        /// <returns>amount of the currency, 0 if not found</returns>
        public int GetAmount(CurrencyData currency)
        {
            return _currencies.TryGetValue(currency, out int amount) ? amount : 0;
        }

        /// <summary>
        /// adds currency to this wallet.
        /// </summary>
        /// <param name="currency">currency type</param>
        /// <param name="amount">amount to add</param>
        public void Add(CurrencyData currency, int amount)
        {
            if (amount <= 0) return;

            if (_currencies.ContainsKey(currency))
                _currencies[currency] += amount;
            else
                _currencies[currency] = amount;
            
            OnCurrencyChanged?.Invoke(currency, _currencies[currency]);
        }

        /// <summary>
        /// removes currency from this wallet.
        /// </summary>
        /// <param name="currency">currency type</param>
        /// <param name="amount">amount to remove</param>
        /// <returns>true if successful</returns>
        public bool Remove(CurrencyData currency, int amount)
        {
            if (GetAmount(currency) < amount) return false;

            _currencies[currency] -= amount;

            // clean up if empty
            if (_currencies[currency] <= 0)
                _currencies.Remove(currency);
            
            OnCurrencyChanged?.Invoke(currency, GetAmount(currency));
            return true;
            
            

        }
    }
}
