using UnityEngine;

/// <summary>
/// Save module for the Player. Saves position and rotation.
/// </summary>
public class PlayerSaveModule : MonoBehaviour, ISaveModule
{
    public string ModuleID => "Player";

    private Transform _player;

    private void Awake()
    {
        // Find player transform
        var playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            _player = playerObj.transform;
            Debug.Log("[PlayerSaveModule] Player found.");
        }
        else
        {
            Debug.LogWarning("[PlayerSaveModule] No GameObject with tag 'Player' found.");
        }

        // Register this module
        SaveRegistry.Register(this);
        Debug.Log("[PlayerSaveModule] Awake → Registered module.");
    }

    [System.Serializable]
    public class PlayerState
    {
        public Vector3 position;
        public Quaternion rotation;
    }

    // Called when saving
    public object CaptureState()
    {
        if (_player == null) return null;

        var state = new PlayerState
        {
            position = _player.position,
            rotation = _player.rotation
        };

        Debug.Log($"[PlayerSaveModule] CaptureState called. Saving position: {_player.position}");
        return state;
    }

    // Called when loading
    public void RestoreState(object stateObj)
    {
        if (_player == null) return;

        var state = stateObj as PlayerState;
        if (state == null) return;

        // Handle CharacterController safely
        var controller = _player.GetComponent<CharacterController>();
        if (controller != null) controller.enabled = false;

        _player.position = state.position;
        _player.rotation = state.rotation;

        if (controller != null) controller.enabled = true;

        Debug.Log($"[PlayerSaveModule] Restored position: {_player.position}");
    }
}
