using UnityEngine;
using TMPro;

namespace Core.Economy
{
    /// <summary>
    /// Handles shop logic:
    /// - Connects to the player's wallet
    /// - Displays currency balances
    /// - Processes purchases of ShopItemData
    /// </summary>
    public class ShopSystem : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Player _player; 

        [Header("UI Output")]
        [SerializeField] private TMP_Text _messageText;

        [Header("Currencies UI")]
        [SerializeField] private CurrencyUI[] _currencyDisplays;

        [Header("Shop Items")]
        [SerializeField] private ShopItemData[] _items;

        private Wallet _wallet; 

        private void Start()
        {
            // Ensure player is assigned
            if (_player == null)
            {
                Debug.LogError("Missing player reference!");
                return;
            }

            // Get player's wallet
            _wallet = _player.GetWallet();
            if (_wallet == null)
            {
                Debug.LogError("Player has no wallet!");
                return;
            }
            
            // Subscribe to wallet updates so UI refreshes automatically
            _wallet.OnCurrencyChanged += HandleCurrencyChanged;
            
            // Initialize UI balances at startup
            UpdateAllBalances();
        }

        private void OnDestroy()
        {
            // Clean up event subscription to avoid memory leaks
            if (_wallet != null)
                _wallet.OnCurrencyChanged -= HandleCurrencyChanged;
        }
        
        /// <summary>
        /// Attempts to buy the given shop item.
        /// Deducts currency if possible, shows success or failure message,
        /// then refreshes balances on screen.
        /// </summary>
        public void Buy(ShopItemData item)
        {
            if (_wallet == null || item == null || item.Currency == null)
            {
                ShowMessage("Setup error: missing wallet or item");
                return;
            }

            // Try to remove currency from wallet
            if (_wallet.Remove(item.Currency, item.Cost))
                ShowMessage(item.SuccessMessage); // success
            else
                ShowMessage($"Not enough {item.Currency.DisplayName}"); // fail

            // Update currency displays regardless of outcome
            UpdateAllBalances();
        }
        
        /// <summary>
        /// Writes a message to the console + UI text field.
        /// </summary>
        private void ShowMessage(string msg)
        {
            Debug.Log($"{msg}");
            if (_messageText != null)
                _messageText.text = msg;
        }

        /// <summary>
        /// Called whenever the wallet changes.
        /// Updates all currency displays.
        /// </summary>
        private void HandleCurrencyChanged(CurrencyData currency, int newAmount)
        {
            foreach (var display in _currencyDisplays)
                display.UpdateDisplay(_wallet);
        }

        /// <summary>
        /// Refreshes all currency balances on screen.
        /// Used at startup and after purchases.
        /// </summary>
        private void UpdateAllBalances()
        {
            foreach (var display in _currencyDisplays)
                display.UpdateDisplay(_wallet);
        }
    }
}
