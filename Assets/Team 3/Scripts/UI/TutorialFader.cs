using System.Collections;

using TMPro;

using UnityEngine;

namespace Project.UI.Tutorials
{
    /// <summary>
    /// Controls the fade-in and fade-out of tutorial messages using a CanvasGroup.
    /// Can dynamically show or hide text messages with smooth fades.
    /// </summary>
    public class TutorialFader : MonoBehaviour
    {
        [Header("References")]
        [Tooltip("CanvasGroup used to control the tutorial UI visibility.")]
        [SerializeField] private CanvasGroup canvasGroup;

        [Tooltip("Text component used to display tutorial messages.")]
        [SerializeField] private TextMeshProUGUI messageText;

        [Header("Timings")]
        [Tooltip("Duration in seconds for the fade-in effect.")]
        [SerializeField] private float fadeInDuration = 0.25f;

        [Tooltip("Duration in seconds for the fade-out effect.")]
        [SerializeField] private float fadeOutDuration = 0.35f;

        private Coroutine fadeRoutine;

        /// <summary>
        /// Automatically resets serialized references in the editor when the component is added.
        /// </summary>
        private void Reset()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            messageText = GetComponent<TextMeshProUGUI>();
        }

        /// <summary>
        /// Initializes the canvas group and ensures it starts fully hidden.
        /// </summary>
        private void Awake()
        {
            if (canvasGroup == null)
            {
                canvasGroup = GetComponent<CanvasGroup>();
            }

            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        /// <summary>
        /// Displays a message and fades in the tutorial UI.
        /// </summary>
        /// <param name="message">The message text to display.</param>
        public void Show(string message)
        {
            messageText.text = message;

            StartFade(1f, fadeInDuration);
        }

        /// <summary>
        /// Fades out the tutorial UI and hides it.
        /// </summary>
        public void Hide()
        {
            StartFade(0f, fadeOutDuration);
        }

        /// <summary>
        /// Starts a fade animation toward the target opacity.
        /// Cancels any ongoing fade before starting a new one.
        /// </summary>
        /// <param name="target">The target alpha value.</param>
        /// <param name="duration">The duration of the fade in seconds.</param>
        private void StartFade(float target, float duration)
        {
            if (fadeRoutine != null)
            {
                StopCoroutine(fadeRoutine);
            }

            fadeRoutine = StartCoroutine(FadeTo(target, duration));
        }

        /// <summary>
        /// Smoothly fades the CanvasGroup alpha over time.
        /// </summary>
        /// <param name="target">Target alpha value.</param>
        /// <param name="duration">Fade duration in seconds.</param>
        private IEnumerator FadeTo(float target, float duration)
        {
            float _startAlpha = canvasGroup.alpha;
            float _time = 0f;

            bool _becomingVisible = target > _startAlpha;
            if (_becomingVisible)
            {
                canvasGroup.blocksRaycasts = true;
                canvasGroup.interactable = false;
            }

            while (_time < duration)
            {
                _time += Time.unscaledDeltaTime;
                float _t = Mathf.Clamp01(_time / duration);
                canvasGroup.alpha = Mathf.Lerp(_startAlpha, target, _t);
                yield return null;
            }

            canvasGroup.alpha = target;

            if (Mathf.Approximately(target, 0f))
            {
                canvasGroup.blocksRaycasts = false;
                canvasGroup.interactable = false;
            }
        }
    }
}
