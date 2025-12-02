using UnityEngine;

public class LockedZone : MonoBehaviour
{
    public GameObject lockedMapIcon;  // Icon that appears on the map

    private bool shown = false;

    void Start()
    {
        lockedMapIcon.SetActive(false); // Hidden until first interaction
    }

    /// <summary>
    /// Call this when player interacts with the locked door/area.
    /// </summary>
    public void TryOpen()
    {
        // First attempt permanently reveals the "Locked" marking
        if (!shown)
        {
            shown = true;
            lockedMapIcon.SetActive(true);
        }

        // Your normal locked door logic here...
    }
}
