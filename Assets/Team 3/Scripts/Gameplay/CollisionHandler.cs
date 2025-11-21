using UnityEngine;

namespace Project.Collision
{
    /// <summary>
    /// Handles collision triggers and logs interactions with objects, 
    /// specifically detecting when the player enters a trigger zone.
    /// </summary>
    public class CollisionHandler : MonoBehaviour
    {
        /// <summary>
        /// Called automatically by Unity when another collider enters this trigger collider.
        /// Logs a message when the player enters or displays the other collider if not the player.
        /// </summary>
        /// <param name="other">The collider that entered the trigger.</param>
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                Debug.Log("Player entered the cube");
            }
            else
            {
                Debug.Log(other.name);
            }
        }
    }
}
