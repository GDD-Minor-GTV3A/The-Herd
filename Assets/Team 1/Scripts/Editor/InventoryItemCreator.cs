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
            item.itemName = $"Trinket{i + 1}";
            item.category = ItemCategory.Trinket;

            // Generate GUID
            string guid = System.Guid.NewGuid().ToString();
            typeof(InventoryItem).GetField("guid",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance)
                .SetValue(item, guid);

            string assetPath = path + item.itemName + ".asset";
            AssetDatabase.CreateAsset(item, assetPath);
            AssetDatabase.SaveAssets();
            Debug.Log($"Created active item: {item.itemName} | GUID: {guid}");
        }

        for (int i = 0; i < 5; i++)
        {
            InventoryItem item = ScriptableObject.CreateInstance<InventoryItem>();
            item.itemName = $"Active_{i + 1}";
            item.category = ItemCategory.Active;

            // Generate GUID
            string guid = System.Guid.NewGuid().ToString();
            typeof(InventoryItem).GetField("guid",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance)
                .SetValue(item, guid);

            string assetPath = path + item.itemName + ".asset";
            AssetDatabase.CreateAsset(item, assetPath);
            AssetDatabase.SaveAssets();
            Debug.Log($"Created active item: {item.itemName} | GUID: {guid}");
        }

        // Create 5 Scroll items
        for (int i = 0; i < 5; i++)
        {
            InventoryItem item = ScriptableObject.CreateInstance<InventoryItem>();
            item.itemName = $"Scroll_{i + 1}";
            item.category = ItemCategory.Scroll;

            // Generate GUID
            string guid = System.Guid.NewGuid().ToString();
            typeof(InventoryItem).GetField("guid",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance)
                .SetValue(item, guid);

            string assetPath = path + item.itemName + ".asset";
            AssetDatabase.CreateAsset(item, assetPath);
            AssetDatabase.SaveAssets();
            Debug.Log($"Created Scroll item: {item.itemName} | GUID: {guid}");
        }

        // Create 5 Revive Totem items
        for (int i = 0; i < 5; i++)
        {
            InventoryItem item = ScriptableObject.CreateInstance<InventoryItem>();
            item.itemName = $"ReviveTotem_{i + 1}";
            item.category = ItemCategory.ReviveTotem;

            // Generate GUID
            string guid = System.Guid.NewGuid().ToString();
            typeof(InventoryItem).GetField("guid",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance)
                .SetValue(item, guid);

            string assetPath = path + item.itemName + ".asset";
            AssetDatabase.CreateAsset(item, assetPath);
            AssetDatabase.SaveAssets();
            Debug.Log($"Created Revive Totem item: {item.itemName} | GUID: {guid}");
        }


        AssetDatabase.Refresh();
        Debug.Log("Test inventory items created!");
    }
}