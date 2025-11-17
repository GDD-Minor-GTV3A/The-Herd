using UnityEngine;
using UnityEditor;

public class InventoryItemCreator
{
    [MenuItem("Inventory/Create Test Items")]
    public static void CreateTestItems()
    {
        string path = "Assets/Resources/InventoryTestItems/";

        // Make sure folder exists
        if (!AssetDatabase.IsValidFolder(path))
            AssetDatabase.CreateFolder("Assets/Resources", "InventoryTestItems");

        // Create 5 wearable items
        for (int i = 0; i < 5; i++)
        {
            InventoryItem item = ScriptableObject.CreateInstance<InventoryItem>();
            item.itemName = $"Wearable_{i + 1}";
            item.category = (ItemCategory)(i % 4); // Head, Chest, Legs, Boots
            item.isActiveItem = false;
            item.activeUses = 0;

            // Generate GUID
            string guid = System.Guid.NewGuid().ToString();
            typeof(InventoryItem).GetField("guid",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance)
                .SetValue(item, guid);

            string assetPath = path + item.itemName + ".asset";
            AssetDatabase.CreateAsset(item, assetPath);
            AssetDatabase.SaveAssets();
            Debug.Log($"Created wearable item: {item.itemName} | GUID: {guid}");
        }

        // Create 5 active items (trinkets)
        for (int i = 0; i < 5; i++)
        {
            InventoryItem item = ScriptableObject.CreateInstance<InventoryItem>();
            item.itemName = $"Active_{i + 1}";
            item.category = ItemCategory.Trinket;
            item.isActiveItem = true;
            item.activeUses = Random.Range(1, 5);

            // Generate GUID
            string guid = System.Guid.NewGuid().ToString();
            typeof(InventoryItem).GetField("guid",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance)
                .SetValue(item, guid);

            string assetPath = path + item.itemName + ".asset";
            AssetDatabase.CreateAsset(item, assetPath);
            AssetDatabase.SaveAssets();
            Debug.Log($"Created active item: {item.itemName} | ActiveUses: {item.activeUses} | GUID: {guid}");
        }

        AssetDatabase.Refresh();
        Debug.Log("Test inventory items created!");
    }
}
