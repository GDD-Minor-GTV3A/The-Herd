using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using Core.Events;
using Core.AI.Sheep.Event;

namespace Core.UI.Sheep
{
    /// <summary>
    /// Manages UI popup for flashbacks
    /// For integration needs to be placed on a Canvas game object in the scene
    /// </summary>
    public class SheepFlahbackManager : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private Image _flashbackImage;
        [SerializeField] private Button _closeButton;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private CanvasGroup[] _otherUIToHide;

        [Header("Settings")] [SerializeField] private float _fadeDuration = 0.3f;
        
        private Action _currentCloseCallback;
        private Coroutine _fadeCoroutine;

        private void Awake()
        {
            if (_canvasGroup == null)
            {
                Debug.LogError("[SheepFlashbackManager] canvasGroup is null");
                return;
            }

            _canvasGroup.alpha = 0f;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;

            if (_closeButton)
            {
                _closeButton.onClick.AddListener(OnCloseButtonClicked);
            }
        }

        private void OnEnable()
        {
            EventManager.AddListener<ShowFlashbackEvent>(OnShowFlashback);
        }

        private void OnDisable()
        {
            EventManager.RemoveListener<ShowFlashbackEvent>(OnShowFlashback);
        }

        private void HideOtherUI()
        {
            if (_otherUIToHide == null) return;
            foreach (var ui in _otherUIToHide)
            {
                if (ui != null)
                {
                    ui.alpha = 0f;
                    ui.interactable = false;
                    ui.blocksRaycasts = false;
                }
            }
        }

        private void ShowOtherUI()
        {
            if (_otherUIToHide == null) return;
            foreach (var ui in _otherUIToHide)
            {
                if (ui != null)
                {
                    ui.alpha = 1f;
                    ui.interactable = true;
                    ui.blocksRaycasts = true;
                }
            }
        }
        
        private void OnShowFlashback(ShowFlashbackEvent evt)
        {
            if (!_canvasGroup) return;

            if (_fadeCoroutine != null) StopCoroutine(_fadeCoroutine);

            if (_flashbackImage)
            {
                _flashbackImage.sprite = evt.FlashbackImage;
                //Disable preserve aspect if needed per image
                _flashbackImage.preserveAspect = true;
            }

            _currentCloseCallback = evt.OnCloseCallback;
            HideOtherUI();
            Time.timeScale = 0f;
            _fadeCoroutine = StartCoroutine(FadeCanvasGroup(_canvasGroup, 1f, _fadeDuration, null));
        }

        private void OnCloseButtonClicked()
        {
            if (_canvasGroup == null) return;
            
            if (_fadeCoroutine != null) StopCoroutine(_fadeCoroutine);
            
            _fadeCoroutine = StartCoroutine(FadeCanvasGroup(_canvasGroup, 0f, _fadeDuration, ResumeGameAndCallback));
        }

        private void ResumeGameAndCallback()
        {
            _currentCloseCallback?.Invoke();
            _currentCloseCallback = null;
            ShowOtherUI();
            Time.timeScale = 1f;
        }
        
        private IEnumerator FadeCanvasGroup(CanvasGroup canvasGroup, float targetAlpha, float duration, Action onComplete = null)
        {
            float startAlpha = canvasGroup.alpha;
            float time = 0f;

            canvasGroup.blocksRaycasts = true;

            while (time < duration)
            {
                time += Time.unscaledDeltaTime;
                float alpha = Mathf.Lerp(startAlpha, targetAlpha, time / duration);
                canvasGroup.alpha = alpha;
                yield return null;
            }
            
            canvasGroup.alpha = targetAlpha;
            bool visible = targetAlpha > 0.01f;
            canvasGroup.blocksRaycasts = visible;
            canvasGroup.interactable = visible;
            
            onComplete?.Invoke();
            _fadeCoroutine = null;
        }
    }
    
}
