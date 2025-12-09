using UnityEngine;
using Gameplay.Player;   // Your Player class is in this namespace

[RequireComponent(typeof(Player))]
public class ForceDeathEvent : MonoBehaviour
{
    [SerializeField]
    private KeyCode key = KeyCode.M;   // change this if you want another key

    private Player player;

    private void Awake()
    {
        player = GetComponent<Player>();
        if (player == null)
        {
            Debug.LogError("[ForceDeathEvent] No Player component found on this GameObject.");
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(key))
        {
            if (player == null)
            {
                Debug.LogError("[ForceDeathEvent] Player reference missing.");
                return;
            }

            Debug.Log("[ForceDeathEvent] Force-death key pressed → calling Player.Die()");
            player.Die();   // This will invoke DeathEvent and reset health
        }
    }
}
