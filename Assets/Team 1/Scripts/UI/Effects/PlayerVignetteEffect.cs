using System.Collections;
using UnityEngine;

namespace UI.Effects 
{
    /// <summary>
    /// Handles logic of custom vignette made for getting damage effect.
    /// </summary>
    public class PlayerVignetteEffect : MonoBehaviour
    {
        [SerializeField, Tooltip("CanvasGroup of vignette. If not assigned, script will try to find one on the object.")] 
        private CanvasGroup canvasGroup;


        private Coroutine showCoroutine;


        /// <summary>
        /// Initialization method.
        /// </summary>
        public void Initialize()
        {
            if (canvasGroup == null && !TryGetComponent<CanvasGroup>(out canvasGroup))
            {
                Debug.LogWarning("PlayerVignetteEffect does NOT have CanvasGroup assigned or found on the object!");
                enabled = false;
                return;
            } 
        }


        /// <summary>
        /// Show vignette for specific duration.Animation separated on 3 parts, each takes the same amount of time(1/3 of full duration). 
        ///1. Fade-in; 2. Vignette full visible; 3.Fade-out.
        /// </summary>
        /// <param name="duration">Duration of full animation.</param>
        public void ShowVignette(float duration)
        {
            if (canvasGroup == null)
                return;

            if (showCoroutine != null)
                StopCoroutine(showCoroutine);
            showCoroutine = StartCoroutine(ShowVignetteRoutine(duration));
        }


        public IEnumerator ShowVignetteRoutine(float duration)
        {
            float _visibleTime = duration / 4;
            float _fadeTime = (duration - _visibleTime) / 2;

            float _currentTime = 0;

            while (_currentTime < _fadeTime)
            {
                _currentTime += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(0, 1, _currentTime / _fadeTime);
                yield return null;
            }

            canvasGroup.alpha = 1;

            yield return new WaitForSeconds(_visibleTime);

            _currentTime = 0;

            while (_currentTime < _fadeTime)
            {
                _currentTime += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(1, 0, _currentTime / _fadeTime);
                yield return null;
            }
        }
    }
}