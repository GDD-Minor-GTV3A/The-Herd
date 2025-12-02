using UnityEngine;

public class MapRevealController : MonoBehaviour
{
    [Header("Reveal Settings")]
    public float revealDistance = 5f;      // Distance from the edge of the revealed area needed to trigger new reveal
    public float revealAmount = 20f;       // How many new units of territory become visible when triggered
    public LayerMask visionBlockMask;      // Obstacles that block map reveal (walls, cliffs, etc.)

    [Header("Reference")]
    public MapData mapData;                // ScriptableObject storing the current map state

    private Transform player;

    void Start()
    {
        // Locate the player (must have the tag "Player")
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        TryReveal();
    }

    /// <summary>
    /// Checks whether the player is close enough to the current map edge.
    /// If yes, attempts to reveal more if not blocked by an obstacle.
    /// </summary>
    void TryReveal()
    {
        Vector2 playerPos = player.position;

        // How far the player is from map center relative to current revealed radius
        float currentRadius = mapData.revealedRadius;

        // If player approaches the boundary (edge - revealDistance)
        if (Vector2.Distance(playerPos, mapData.centerPosition) >= currentRadius - revealDistance)
        {
            // Direction from center toward player
            Vector2 dir = (playerPos - mapData.centerPosition).normalized;

            // Check if any obstacle blocks sight beyond current map radius
            if (!Physics2D.Raycast(mapData.centerPosition, dir, revealAmount, visionBlockMask))
            {
                // Reveal more territory
                mapData.RevealMore(revealAmount);
            }
        }
    }
}
