namespace Core.Economy
{
    /// <summary>
    /// trading system for transferring currencies between wallets.
    /// </summary>
    public static class Trading
    {
        /// <summary>
        /// transfers currency from one wallet to another.
        /// </summary>
        /// <param name="from">wallet to transfer from</param>
        /// <param name="to">wallet to transfer to</param>
        /// <param name="currency">currency type</param>
        /// <param name="amount">amount to transfer</param>
        /// <returns>true if transfer was successful</returns>
        public static bool Transfer(Wallet from, Wallet to, CurrencyData currency, int amount)
        {
            if (from == null || to == null || currency == null || amount <= 0)
                return false;

            // check if source has enough
            if (from.GetAmount(currency) < amount)
                return false;

            // do the transfer
            if (from.Remove(currency, amount))
            {
                to.Add(currency, amount);
                return true;
            }

            return false;
        }
    }
}
