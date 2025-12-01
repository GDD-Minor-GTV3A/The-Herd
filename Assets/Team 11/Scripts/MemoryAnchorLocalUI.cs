using System.Collections;
using UnityEngine;
using TMPro;

public class MemoryAnchorLocalUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private float showDuration = 2f;
    [SerializeField] private float fadeDuration = 0.25f;

    private Coroutine routine;

    private void Awake()
    {
        if (canvasGroup != null)
        {
            // Keep the GameObject active, just make it invisible
            canvasGroup.alpha = 0f;
        }
    }

    public void Show(string text)
    {
        if (messageText != null)
        {
            messageText.text = string.IsNullOrWhiteSpace(text)
                ? "Memory Anchor discovered"
                : text;
        }

        if (routine != null)
        {
            StopCoroutine(routine);
        }

        routine = StartCoroutine(ShowRoutine());
    }

    private IEnumerator ShowRoutine()
    {
        if (canvasGroup == null)
            yield break;

        // Fade in
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, t / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 1f;

        // Stay visible
        yield return new WaitForSeconds(showDuration);

        // Fade out
        t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, t / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 0f;

        routine = null;
    }
}
