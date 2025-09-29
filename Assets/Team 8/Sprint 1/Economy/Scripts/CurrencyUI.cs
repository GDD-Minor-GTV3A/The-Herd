using UnityEngine;
using TMPro;

namespace Core.Economy
{
    [System.Serializable]
    public class CurrencyUI
    {
        [SerializeField] private CurrencyData _currency;
        [SerializeField] private TMP_Text _text;

        public void UpdateDisplay(Wallet wallet)
        {
            if (_currency == null || _text == null || wallet == null) return;
            _text.text = $"{wallet.GetAmount(_currency)}";
        }
    }
}