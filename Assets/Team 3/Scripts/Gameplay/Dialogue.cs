using System.Collections;
using UnityEngine;
using TMPro;

namespace Project.UI
{
    /// <summary>
    /// Handles dialogue text display by typing each character over time.
    /// Allows skipping or advancing dialogue lines with mouse input.
    /// </summary>
    public class Dialogue : MonoBehaviour
    {
        [Header("UI Components")]
        [Tooltip("Text component used to display dialogue.")]
        [SerializeField] private TextMeshProUGUI textComponent;

        [Header("Dialogue Settings")]
        [Tooltip("Lines of dialogue to display in sequence.")]
        [SerializeField] private string[] lines;

        [Tooltip("Delay in seconds between each character.")]
        [Range(0.01f, 0.5f)]
        [SerializeField] private float textSpeed = 0.05f;

        private int currentIndex;

        /// <summary>
        /// Initializes the dialogue by clearing text and starting the first line.
        /// </summary>
        private void Start()
        {
            textComponent.text = string.Empty;
            StartDialogue();
        }

        /// <summary>
        /// Checks for mouse input to skip or advance dialogue lines.
        /// </summary>
        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (textComponent.text == lines[currentIndex])
                {
                    NextLine();
                }
                else
                {
                    StopAllCoroutines();
                    textComponent.text = lines[currentIndex];
                }
            }
        }

        /// <summary>
        /// Starts displaying the dialogue from the first line.
        /// </summary>
        private void StartDialogue()
        {
            currentIndex = 0;
            StartCoroutine(TypeLine());
        }

        /// <summary>
        /// Types out each character of the current dialogue line over time.
        /// </summary>
        private IEnumerator TypeLine()
        {
            foreach (char _character in lines[currentIndex].ToCharArray())
            {
                textComponent.text += _character;
                yield return new WaitForSeconds(textSpeed);
            }
        }

        /// <summary>
        /// Advances to the next line or hides the dialogue box when finished.
        /// </summary>
        private void NextLine()
        {
            if (currentIndex < lines.Length - 1)
            {
                currentIndex++;
                textComponent.text = string.Empty;
                StartCoroutine(TypeLine());
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }
}
