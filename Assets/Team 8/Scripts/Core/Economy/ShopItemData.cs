using UnityEngine;

namespace Core.Economy
{
    /// <summary>
    /// Defines a shop item that can be bought with a specific currency.
    /// </summary>
    [CreateAssetMenu(fileName = "ShopItem", menuName = "Economy/Shop Item")]
    public class ShopItemData : ScriptableObject
    {
        [SerializeField] private string _itemName;
        [SerializeField] private CurrencyData _currency;
        [SerializeField] private int _cost;
        [SerializeField] private string _successMessage;

        public string ItemName => _itemName;
        public CurrencyData Currency => _currency;
        public int Cost => _cost;
        public string SuccessMessage => _successMessage;
    }
}