using UnityEngine;

namespace Core.Economy
{
    /// <summary>
    /// scriptable object that defines a currency type in the game.
    /// </summary>
    [CreateAssetMenu(fileName = "New Currency", menuName = "Economy/Currency")]
    public class CurrencyData : ScriptableObject
    {
        [SerializeField] private string _displayName;
        [SerializeField] private Sprite _icon;

        /// <summary>
        /// display name for this currency.
        /// </summary>
        public string DisplayName => _displayName;

        /// <summary>
        /// icon sprite for UI display.
        /// </summary>
        public Sprite Icon => _icon;

        private void OnValidate()
        {
            // ensure display name is not empty
            if (string.IsNullOrEmpty(_displayName))
                _displayName = name;
        }
    }
}
