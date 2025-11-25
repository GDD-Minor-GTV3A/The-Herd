using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using System.Collections.Generic;

public class InventoryItemCreator
{
    [MenuItem("Inventory/Create Test Items")]
    public static void CreateTestItems()
    {
        string path = "Assets/Resources/InventoryTestItems/";

        // Make sure folder exists
        if (!AssetDatabase.IsValidFolder(path))
            AssetDatabase.CreateFolder("Assets/Resources", "InventoryTestItems");

        // Load all sprites from your folder
        string spriteFolderPath = "Assets/Team 1/Scripts/UI/Artwok/UI";
        string[] spriteFiles = Directory.GetFiles(spriteFolderPath, "*.png", SearchOption.AllDirectories);
        List<Sprite> sprites = new List<Sprite>();
        foreach (var file in spriteFiles)
        {
            string assetPath = file.Replace("\\", "/"); // Ensure proper path format
            Sprite[] loaded = AssetDatabase.LoadAllAssetsAtPath(assetPath).OfType<Sprite>().ToArray();
            if (loaded != null) sprites.AddRange(loaded);
        }

        // Helper function to get random sprite
        Sprite GetRandomSprite() => sprites.Count > 0 ? sprites[Random.Range(0, sprites.Count)] : null;

        // Create 5 wearable items
        for (int i = 0; i < 5; i++)
        {
            InventoryItem item = ScriptableObject.CreateInstance<InventoryItem>();
            item.itemName = $"Wearable_{i + 1}";
            item.category = (ItemCategory)(i % 4); // Head, Chest, Legs, Boots
            item.icon = GetRandomSprite();

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

        // Create 5 trinkets
        for (int i = 0; i < 5; i++)
        {
            InventoryItem item = ScriptableObject.CreateInstance<InventoryItem>();
            item.itemName = $"Trinket{i + 1}";
            item.category = ItemCategory.Trinket;
            item.icon = GetRandomSprite();

            string guid = System.Guid.NewGuid().ToString();
            typeof(InventoryItem).GetField("guid",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance)
                .SetValue(item, guid);

            string assetPath = path + item.itemName + ".asset";
            AssetDatabase.CreateAsset(item, assetPath);
            AssetDatabase.SaveAssets();
            Debug.Log($"Created trinket item: {item.itemName} | GUID: {guid}");
        }

        // Create 5 active items
        for (int i = 0; i < 5; i++)
        {
            InventoryItem item = ScriptableObject.CreateInstance<InventoryItem>();
            item.itemName = $"Active_{i + 1}";
            item.category = ItemCategory.Active;
            item.icon = GetRandomSprite();

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

        // Create 5 scrolls
        for (int i = 0; i < 5; i++)
        {
            InventoryItem item = ScriptableObject.CreateInstance<InventoryItem>();
            item.itemName = $"Scroll_{i + 1}";
            item.category = ItemCategory.Scroll;
            item.icon = GetRandomSprite();

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

        // Create 5 revive totems
        for (int i = 0; i < 5; i++)
        {
            InventoryItem item = ScriptableObject.CreateInstance<InventoryItem>();
            item.itemName = $"ReviveTotem_{i + 1}";
            item.category = ItemCategory.ReviveTotem;
            item.icon = GetRandomSprite();

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
        Debug.Log("Test inventory items created with random sprites!");
    }
}
