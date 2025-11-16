using UnityEngine;

/// <summary>
/// ScriptableObject holding dynamic data about map progress.
/// </summary>
[CreateAssetMenu(menuName = "Map/MapData")]
public class MapData : ScriptableObject
{
    [Header("Map State")]
    public Vector2 centerPosition;     // Starting reference point (often level start)
    public float revealedRadius = 0f;  // How much of the map is currently revealed

    // Event triggered whenever new territory is revealed
    public delegate void RevealEvent();
    public event RevealEvent OnReveal;

    /// <summary>
    /// Increases revealed radius and notifies listeners.
    /// </summary>
    public void RevealMore(float amount)
    {
        revealedRadius += amount;

        // Notify UI or other listeners
        OnReveal?.Invoke();
    }
}
