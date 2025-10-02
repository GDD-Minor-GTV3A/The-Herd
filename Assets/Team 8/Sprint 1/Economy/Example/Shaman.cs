using Core.Economy;

using UnityEngine;

public class Shaman : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Player _player;

    [Header("Shaman Currencies")]
    [SerializeField] private CurrencyData _currency;

    private Wallet _wallet;

    void Start()
    {
        _wallet = new Wallet();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            Trade();
        }
    }

    public void Trade()
    {
        const int amount = 25;

        bool success = Trading.Transfer(
            _player.GetWallet(),
            _wallet,
            _currency,
            amount
        );

        if (success)
        {
            Debug.Log($"Trade successful ! {amount} {_currency.DisplayName}");
            Debug.Log($"Shaman now has: {_wallet.GetAmount(_currency)} {_currency.DisplayName}");
            Debug.Log($"Player now has: {_player.GetWallet().GetAmount(_currency)} {_currency.DisplayName}");
        }
        else
        {
            Debug.Log("Trade failed - not enough " + _currency.DisplayName);
            Debug.Log($"Shaman now has: {_wallet.GetAmount(_currency)} {_currency.DisplayName}");
            Debug.Log($"Player now has: {_player.GetWallet().GetAmount(_currency)} {_currency.DisplayName}");
        }
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 300, 20), "T=Trade | G=Give Currency");

        if (_player != null && _currency != null)
        {
            int playerCurrency = _player.GetWallet().GetAmount(_currency);
            int shopCurrency = _wallet.GetAmount(_currency);
            GUI.Label(new Rect(10, 30, 300, 20), $"Player: {playerCurrency} currency | Shop: {shopCurrency} currency");
        }
    }
}