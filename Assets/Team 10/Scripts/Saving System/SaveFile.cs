using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// The container for all saved data. Do not touch please.
/// </summary>
[System.Serializable]
public class SaveFile
{
    public List<ModuleData> modules = new List<ModuleData>();
}

[System.Serializable]
public class ModuleData
{
    public string moduleID;
    public string jsonData; // store each module as JSON string
}
