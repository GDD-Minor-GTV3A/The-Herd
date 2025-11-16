using UnityEngine;

public class POIReveal : MonoBehaviour
{
    public float revealDistance = 5f;  // Distance needed to reveal this POI
    public GameObject mapIcon;         // Icon on the map UI

    private Transform player;
    private bool revealed = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        // Icon should be invisible until discovered
        mapIcon.SetActive(false);
    }

    void Update()
    {
        // Reveal when player is within distance
        if (!revealed && Vector2.Distance(player.position, transform.position) < revealDistance)
        {
            revealed = true;
            mapIcon.SetActive(true);  // Now visible on map
        }
    }
}
