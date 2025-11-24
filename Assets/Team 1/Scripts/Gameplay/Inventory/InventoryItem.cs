using CustomEditor.Attributes;

using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Inventory/Item")]
public class InventoryItem : ScriptableObject
{
    [Header("Identity")]
    public string itemName;
    public Sprite icon;

    [Header("Category")]
    public ItemCategory category;

    [Header("Stats (optional)")]
    public int bonusHealth;
    public int bonusDamage;
    public int bonusSpeed;

    [Header("Save")]
    [Tooltip("Auto-generated unique ID used for saving. Keep hidden.")]
    [SerializeField] private string guid;

    public string GUID => guid;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (string.IsNullOrEmpty(guid))
            guid = System.Guid.NewGuid().ToString();
    }
#endif
}