using Core.Economy;

using UnityEngine;

public class PlayerWallet : MonoBehaviour
{
    [Header("Player Currencies")]
    [SerializeField] private CurrencyData _currency1;
    [SerializeField] private CurrencyData _currency2;

    private Wallet _wallet;

    void Awake()
    {
        _wallet = new Wallet();

        if (_currency1 != null)
            _wallet.Add(_currency1, 100);
        if (_currency2 != null)
            _wallet.Add(_currency2, 50);

    }

    public Wallet GetWallet()
    {
        return _wallet;
    }
}