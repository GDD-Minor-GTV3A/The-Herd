using UnityEngine;
using System.IO;

/// <summary>
/// SaveSystem is responsible for actually saving and loading the game to/from a file.
/// 
/// It collects data from all registered Save Modules (via SaveRegistry),
/// writes it to a JSON file, and can also read a file to restore the saved data.
/// 
/// Key points:
/// - You do NOT put this on a GameObject; it’s a static utility class.
/// - It works with all modules registered in SaveRegistry.
/// - You call SaveGame(path) or LoadGame(path) when you want to save/load.
/// </summary>
public static class SaveSystem
{
    /// <summary>
    /// Saves the current game state to a file.
    /// </summary>
    public static void SaveGame(string filePath)
    {
        // Create a new SaveFile container to hold all module data
        SaveFile saveFile = new SaveFile();

        // Ask every registered module for its data
        foreach (var module in SaveRegistry.Modules.Values)
        {
            saveFile.data[module.ModuleID] = module.CaptureState();
        }

        // Convert the SaveFile to JSON text
        string json = JsonUtility.ToJson(saveFile, prettyPrint: true);

        // Write the JSON to a file
        File.WriteAllText(filePath, json);

        Debug.Log($"Game saved to {filePath}");
    }

    /// <summary>
    /// Loads a saved game from a file and restores all module states.
    /// </summary>
    public static void LoadGame(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Debug.LogWarning($"No save file found at {filePath}");
            return;
        }

        // Read the JSON from the file
        string json = File.ReadAllText(filePath);

        // Convert JSON back into a SaveFile object
        SaveFile saveFile = JsonUtility.FromJson<SaveFile>(json);

        // Give each module its own saved data
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
