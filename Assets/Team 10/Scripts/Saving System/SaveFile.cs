using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// The container for all saved data. Do not touch please.
/// </summary>
[Serializable]
public class SaveFile
{
    public Dictionary<string, object> data = new();
}
