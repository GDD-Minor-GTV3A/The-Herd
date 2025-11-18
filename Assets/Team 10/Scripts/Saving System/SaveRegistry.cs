using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// SaveRegistry keeps track of all Save Modules in the game.
/// 
/// Every system that wants to save data must register its module here.
/// Each module must have a unique ModuleID.
/// 
/// Key points:
/// - Uses a dictionary so each ModuleID is unique.
/// - No GameObject needed; no inspector setup.
/// - Must call Register() for your module (usually in Awake()).
/// </summary>
public static class SaveRegistry
{
    // Dictionary of all registered modules
    private static readonly Dictionary<string, ISaveModule> modules = new Dictionary<string, ISaveModule>();

    // Add a module to the registry
    public static void Register(ISaveModule module)
    {
        if (modules.ContainsKey(module.ModuleID))
        {
            Debug.LogWarning(
                $"Save module with ID '{module.ModuleID}' is already registered."
            );
        }

        modules[module.ModuleID] = module;
    }

    // Gives access to all registered modules (read-only)
    public static IReadOnlyDictionary<string, ISaveModule> Modules => modules;
}
