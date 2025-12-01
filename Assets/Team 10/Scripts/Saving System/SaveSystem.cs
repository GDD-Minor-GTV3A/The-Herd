using UnityEngine;
using System.IO;



public static class SaveSystem
{
    public static void SaveGame(string filePath)
    {
        SaveFile saveFile = new SaveFile();

        // Collect state from every module
        foreach (var module in SaveRegistry.Modules.Values)
        {
            saveFile.data[module.ModuleID] = module.CaptureState();
        }

        string json = JsonUtility.ToJson(saveFile, prettyPrint: true);
        File.WriteAllText(filePath, json);

        Debug.Log($"Game saved to {filePath}");
    }

    public static void LoadGame(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Debug.LogWarning($"No save file found at {filePath}");
            return;
        }

        string json = File.ReadAllText(filePath);
        SaveFile saveFile = JsonUtility.FromJson<SaveFile>(json);

        // Hand each module its own saved data
        foreach (var kvp in saveFile.data)
        {
            if (SaveRegistry.Modules.TryGetValue(kvp.Key, out var module))
            {
                module.RestoreState(kvp.Value);
            }
        }

        Debug.Log($"Game loaded from {filePath}");
    }
}
