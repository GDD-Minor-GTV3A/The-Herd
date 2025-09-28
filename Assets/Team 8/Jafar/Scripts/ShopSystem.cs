using UnityEngine;
using TMPro;

namespace Core.Economy
{
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
            if (_player == null)
            {
                Debug.LogError("Missing player reference!");
                return;
            }

            _wallet = _player.GetWallet();
            if (_wallet == null)
            {
                Debug.LogError("Player has no wallet!");
                return;
            }
            
            _wallet.OnCurrencyChanged += HandleCurrencyChanged;
            
            UpdateAllBalances();
        }

        private void OnDestroy()
        {
            if (_wallet != null)
                _wallet.OnCurrencyChanged -= HandleCurrencyChanged;
        }
        
        public void Buy(ShopItemData item)
        {
            if (_wallet == null || item == null || item.Currency == null)
            {
                ShowMessage("Setup error: missing wallet or item");
                return;
            }

            if (_wallet.Remove(item.Currency, item.Cost))
                ShowMessage(item.SuccessMessage);
            else
                ShowMessage($"Not enough {item.Currency.DisplayName}");

            UpdateAllBalances();
        }
        
        private void ShowMessage(string msg)
        {
            Debug.Log($"{msg}");
            if (_messageText != null)
                _messageText.text = msg;
        }

        private void HandleCurrencyChanged(CurrencyData currency, int newAmount)
        {
            foreach (var display in _currencyDisplays)
                display.UpdateDisplay(_wallet);
        }

        private void UpdateAllBalances()
        {
            foreach (var display in _currencyDisplays)
                display.UpdateDisplay(_wallet);
        }
    }
}
