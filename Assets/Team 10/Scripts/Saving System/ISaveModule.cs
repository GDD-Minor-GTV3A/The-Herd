using UnityEngine;

/// <summary>
/// This interface is the *required template* for every Save Module.
/// 
/// An interface is like a checklist: any script that uses it MUST
/// include the functions listed here.
/// 
/// Every team will create their own Save Module by implementing this
/// interface. This ensures all modules can:
/// 
/// 1. Provide a unique ID (ModuleID)
/// 2. Save their data (CaptureState)
/// 3. Load their data (RestoreState)
/// 
/// The interface itself does NOT contain any saving logic.
/// It only defines what functions must exist.
/// </summary>
/// 
public interface ISaveModule
{
    // A unique name for this module (e.g., "Player", "Sheep", "Quests")
    string ModuleID { get; }

    // Returns all the data this module wants to save
    object CaptureState();

    // Receives previously saved data and restores it
    void RestoreState(object state);
}


/// <summary>
/// Example of how to implement this interface in your own module:
/// </summary>
///
/// public class PlayerSaveModule : ISaveModule
/// {
///     public string ModuleID => "Player";
///
///     public object CaptureState()
///     {
///         // return the data you want to save
///     }
///
///     public void RestoreState(object state)
///     {
///         // take the saved data and apply it back to your systems
///     }
/// }
