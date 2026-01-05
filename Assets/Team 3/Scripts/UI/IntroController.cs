using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

namespace Project.UI.Tutorials
{
    /// <summary>
    /// Handles the intro sequence by playing the intro audio and fadeing the black screen after the audio has played
    /// </summary>
    public class IntroController : MonoBehaviour
    {
        [Header("UI")]
        [Tooltip("Image used as a full-screen black overlay.")]
        [SerializeField] private Image fadeImage;

        [Header("Timings")]
        [Tooltip("Time in seconds before the fade-out starts.")]
        [SerializeField] private float waitBeforeFade = 32f;

        [Tooltip("Duration in seconds of the fade-out.")]
        [SerializeField] private float fadeDuration = 5f;

        [Header("Skip Intro")]
        [Tooltip("Time in seconds the spacebar must be held to skip the intro.")]
        [SerializeField] private float skipHoldDuration = 1f;

        [Header("Skip Intro UI")]
        [Tooltip("Text shown to inform the player how to skip the intro.")]
        [SerializeField] private TextMeshProUGUI skipIntroText;

        [Header("Story based message")]
        [SerializeField] private TextMeshProUGUI storyMessage;

        private Coroutine fadeRoutine;
        private float skipHoldTimer;
        private bool isFading;
        private AudioSource introAudioSource;

        /// <summary>
        /// Starts the intro sequence.
        /// </summary>
        private void Start()
        {
            introAudioSource = GetComponent<AudioSource>();

            fadeRoutine = StartCoroutine(FadeSequence());
        }

        /// <summary>
        /// Checks if the spacebar is held long enough to skip the intro.
        /// </summary>
        private void Update()
        {
            if (isFading)
            {
                return;
            }

            if (Input.GetKey(KeyCode.Space))
            {
                skipHoldTimer += Time.deltaTime;

                if (skipHoldTimer >= skipHoldDuration)
                {
                    SkipIntro();
                }
            }
            else
            {
                skipHoldTimer = 0f;
            }
        }

        /// <summary>
        /// Immediately skips the intro and starts the fade-out.
        /// </summary>
        private void SkipIntro()
        {
            if (fadeRoutine != null)
            {
                StopCoroutine(fadeRoutine);
            }

            StopIntroAudio();
            StartCoroutine(FadeOut());
        }

        /// <summary>
        /// Waits for the intro duration before starting the fade-out.
        /// </summary>
        private IEnumerator FadeSequence()
        {
            yield return new WaitForSeconds(waitBeforeFade);

            StopIntroAudio();
            yield return FadeOut();
        }

        /// <summary>
        /// Stops the intro audio if it is playing.
        /// </summary>
        private void StopIntroAudio()
        {
            if (introAudioSource != null && introAudioSource.isPlaying)
            {
                introAudioSource.Stop();
            }
        }

        /// <summary>
        /// Fades out the black screen.
        /// </summary>
        private IEnumerator FadeOut()
        {
            isFading = true;

            if (skipIntroText != null)
            {
                skipIntroText.gameObject.SetActive(false);
            }

            float _time = 0f;
            Color _color = fadeImage.color;

            while (_time < fadeDuration)
            {
                _time += Time.deltaTime;
                _color.a = Mathf.Lerp(1f, 0f, _time / fadeDuration);
                fadeImage.color = _color;
                yield return null;
            }

            _color.a = 0f;
            fadeImage.color = _color;

            fadeImage.gameObject.SetActive(false);

            if (storyMessage != null)
            {
                storyMessage.gameObject.SetActive(false);
            }
        }
    }
}
