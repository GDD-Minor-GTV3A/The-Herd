using UnityEngine;

namespace Gameplay.Inventory
{
    [CreateAssetMenu(fileName = "Item", menuName = "Inventory/Item")]
    public class InventoryItem : ScriptableObject
    {
        [System.Serializable]
        public class ItemStats
        {
            [SerializeField] private int bonusHealth;
            [SerializeField] private int bonusDamage;
            [SerializeField] private int bonusSpeed;
            [SerializeField] private float playerVisionRange;
            [SerializeField] private float dogVisionRange;

            public int BonusHealth { get => bonusHealth; set => bonusHealth = value; }
            public int BonusDamage { get => bonusDamage; set => bonusDamage = value; }
            public int BonusSpeed { get => bonusSpeed; set => bonusSpeed = value; }

            public float PlayerVisionRange { get => playerVisionRange; set => playerVisionRange = value; }

            public float DogVisionRange { get => dogVisionRange; set => dogVisionRange = value; }
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