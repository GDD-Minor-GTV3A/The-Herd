using UnityEngine;
using System.Collections.Generic;

public static class SaveRegistry
{
    private static readonly Dictionary<string, ISaveModule> modules = new();

    public static void Register(ISaveModule module)
    {
        if (modules.ContainsKey(module.ModuleID))
        {
            Debug.LogWarning($"[SaveRegistry] Module '{module.ModuleID}' already registered — replacing.");
        }
        else
        {
            Debug.Log($"[SaveRegistry] Registered module '{module.ModuleID}'.");
        }

        modules[module.ModuleID] = module;
    }

    public static IReadOnlyDictionary<string, ISaveModule> Modules => modules;
}
