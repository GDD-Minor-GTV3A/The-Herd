using UnityEngine;
using Core.Economy;

public class GetCurrency : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerWallet _player;

    [Header("Currencies to Give")]
    [SerializeField] private CurrencyData _currency1;
    [SerializeField] private CurrencyData _currency2;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            GiveCurrency(_currency1, 50);
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            GiveCurrency(_currency2, 50);
        }
    }

    private void GiveCurrency(CurrencyData currency, int amount)
    {
        if (_player == null || currency == null)
        {
            Debug.LogWarning("Player or Currency not assigned!");
            return;
        }

        _player.GetWallet().Add(currency, amount);
        Debug.Log($"Given {amount} {currency.DisplayName} to player. " +
                  $"Player now has: {_player.GetWallet().GetAmount(currency)} {currency.DisplayName}");
    }
}