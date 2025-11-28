using UnityEngine;

namespace Gameplay.Inventory
{
    [CreateAssetMenu(fileName = "Item", menuName = "Inventory/Item")]
    public class InventoryItem : ScriptableObject
    {
        [System.Serializable]
        public class ItemStats
        {
            public int bonusHealth;
            public int bonusDamage;
            public int bonusSpeed;
        }

        [Header("Identity")]
        public string itemName;
        public Sprite icon;

        [Header("Category")]
        public ItemCategory category;

        [Header("Stats")]
        public ItemStats stats;

        [Header("Stacking")]
        public bool stackable = false;
        public int maxStack = 99;

        [SerializeField, HideInInspector]
        private string guid;
        public string GUID => guid;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (string.IsNullOrEmpty(guid))
                guid = System.Guid.NewGuid().ToString();
        }
#endif
    }
}