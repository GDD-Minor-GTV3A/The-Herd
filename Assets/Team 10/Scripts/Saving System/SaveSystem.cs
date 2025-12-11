using System.IO;
using System.Collections.Generic;
using UnityEngine;

public static class SaveSystem
{
    public static void SaveGame(string filePath)
    {
        SaveFile saveFile = new SaveFile();

        foreach (var module in SaveRegistry.Modules.Values)
        {
            object state = module.CaptureState();
            if (state == null) continue;

            string moduleJson = JsonUtility.ToJson(state);
            saveFile.modules.Add(new ModuleData
            {
                moduleID = module.ModuleID,
                jsonData = moduleJson
            });

            Debug.Log($"[SaveSystem] Captured module '{module.ModuleID}': {moduleJson}");
        }

        string json = JsonUtility.ToJson(saveFile, true);
        File.WriteAllText(filePath, json);
        Debug.Log($"[SaveSystem] Save complete → {filePath}\n{json}");
    }

    public static void LoadGame(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Debug.LogWarning($"No save file at {filePath}");
            return;
        }

        string json = File.ReadAllText(filePath);
        SaveFile saveFile = JsonUtility.FromJson<SaveFile>(json);

        foreach (var data in saveFile.modules)
        {
            if (SaveRegistry.Modules.TryGetValue(data.moduleID, out var module))
            {
                var type = module.CaptureState()?.GetType();
                if (type != null)
                {
                    object state = JsonUtility.FromJson(data.jsonData, type);
                    module.RestoreState(state);
                    Debug.Log($"[SaveSystem] Restored module '{module.ModuleID}' → {data.jsonData}");
                }
            }
        }

        Debug.Log("[SaveSystem] Load complete");
    }
}
