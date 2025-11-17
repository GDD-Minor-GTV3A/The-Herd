using UnityEngine;

/// <summary>
/// TEMPLATE SAVE MODULE
/// ---------------------
/// Copy this file and rename the class for your own system.
/// Implement ISaveModule to allow the SaveSystem to save/load your data.
/// 
/// Key points:
/// - Each module must have a unique ModuleID
/// - CaptureState() returns all the data you want to save
/// - RestoreState() restores the data when loading
/// - Do NOT put this on a GameObject
/// - Do NOT modify SaveSystem.cs
/// </summary>
public class TemplateSaveModule : ISaveModule
{
    // Unique ID for this module
    public string ModuleID => "TemplateModule";

    // Called when saving: return all data you want to save
    public object CaptureState()
    {
        // Example: return a simple data structure
        return new TemplateState
        {
            exampleNumber = 0,
            exampleText = "Hello"
        };
    }

    // Called when loading: restore data from saved object
    public void RestoreState(object stateObj)
    {
        var state = stateObj as TemplateState;
        if (state == null) return;

        // Restore your data here
        Debug.Log($"Restored exampleNumber: {state.exampleNumber}, exampleText: {state.exampleText}");
    }

    // Example of a serializable state class
    [System.Serializable]
    public class TemplateState
    {
        public int exampleNumber;
        public string exampleText;
    }
}
