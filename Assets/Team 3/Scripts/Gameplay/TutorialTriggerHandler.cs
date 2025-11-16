using UnityEngine;

namespace Project.UI.Tutorials
{
    /// <summary>
    /// Detects when the player enters or exits a trigger zone and displays or hides a tutorial message.
    /// </summary>
    public class TutorialTriggerHandler : MonoBehaviour
    {
        [Header("Tutorial Message")]
        [Tooltip("The text message shown to the player when entering this trigger area.")]
        [TextArea(2, 4)]
        [SerializeField] private string tutorialMessage;

        [Header("References")]
        [Tooltip("Reference to the TutorialFader component that controls the message display.")]
        [SerializeField] private TutorialFader fader;

        /// <summary>
        /// Automatically finds a <see cref="TutorialFader"/> in the scene if none is assigned.
        /// </summary>
        private void Reset()
        {
            if (fader == null)
            {
                fader = FindFirstObjectByType<TutorialFader>();
            }
        }

        /// <summary>
        /// Called when another collider enters this trigger zone.
        /// Displays the tutorial message if the collider is the player
        /// </summary>
        /// <param name="other">The collider that entered the trigger.</param>
        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player"))
            {
                return;
            }

            if (fader != null)
            {
                fader.Show(tutorialMessage);
            }
        }

        /// <summary>
        /// Called when another collider exits this trigger zone.
        /// Hides the tutorial message if the collider is the player.
        /// </summary>
        /// <param name="other">The collider that exited the trigger.</param>
        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Player"))
            {  
                return; 
            }
               
            if (fader != null)
            {
                fader.Hide();
            }
        }
    }
}
