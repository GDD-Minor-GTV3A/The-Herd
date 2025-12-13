# Trading System

Generic currency system for trading.

## Integration

To integrate the Trading System in the current codebase :
1. Create the folder `Economy` at `Assets/_Game/Scripts/Core/`
2. `CurrencyData.cs` should go in `Assets/_Game/Scripts/Core/Economy/`
3. `Trading.cs` should go in `Assets/_Game/Scripts/Core/Economy/`
4. `Wallet.cs` should go in `Assets/_Game/Scripts/Core/Economy/`

## Components

- `CurrencyData`: Generic type representing any tradable object  
- `Wallet`: Container that holds different currencies  
- `Trading`: Static class to transfer currency  

## Usage

> [!NOTE]
> In this usage example we'll talk about **gold** as the main currency, but it obviously work with everything.

Create currency types in Unity (Create > Economy > Currency), name it `Gold` and in the inspector add
the **Diplay Name** and the resource **Icon**.

Then in the player you could do something like that :
```csharp
[Header("Player Currencies")]
[SerializeField] private CurrencyData _goldCurrency;

private Wallet _wallet;


public void Initialize()
{
    var _wallet = new Wallet();

    // you can add initial amount if necessary
    if (_goldCurrency != null)
        _wallet.Add(_goldCurrency, 100);
}
```

And the shaman would have :

```csharp
[Header("References")]
[SerializeField] private Player _player; // store a reference of the player to access its wallet

[Header("Shaman Currencies")]
[SerializeField] private CurrencyData _goldCurrency;

private Wallet _wallet;

void Initialize()
{
    _wallet = new Wallet();
}

public void Trade()
{
    const int amount = 25;

    bool success = Trading.Transfer(
        _player.GetWallet(),
        _wallet,
        _goldCurrency,
        amount
    );

    if (success)
    {
        // do something...
    }
    else
    {
        // do something else...
    }
}
```

> [!WARNING]
> When using this system, make sure that you don't forget any references to the player or currencies

> [!TIP]
> For a complete - still basic - working example, see `Example/`
