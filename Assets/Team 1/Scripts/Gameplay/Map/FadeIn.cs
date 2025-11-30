using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq.Expressions;

public class FadeChildrenImages : MonoBehaviour
{
    public float duration = 1f;
    private Image[] images;
    private bool hasfaded = false;  
    private float posX;
    private float posY;
    private float width;
    private float height;
    private float widthHalf;
    private float heightHalf;
    private CanvasGroup cg;
    private Coroutine fadeRoutine;

    void Awake()
    {
        images = GetComponentsInChildren<Image>(true); // include inactive children
        RectTransform rt = GetComponent<RectTransform>();


        posX = rt.anchoredPosition.x;
        posY = rt.anchoredPosition.y;
        width = rt.rect.width;
        height = rt.rect.height;
        widthHalf = width / 2f;
        heightHalf = height / 2f;

        cg = GetComponent<CanvasGroup>();
    }
    
    public void FadeIn()
    {
        if (hasfaded) return; // prevent multiple fades


        // initialize all image alphas to 0 BEFORE the first frame is visible
        foreach (var img in images)
        {
            Color c = img.color;
            c.a = 0f;
            img.color = c;
        }

        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        fadeRoutine = StartCoroutine(FadeInRoutine());
    }
    private IEnumerator FadeInRoutine()
    {

        hasfaded = true;

        // Ensure CanvasGroup doesn't block visibility
        if (cg != null)
        {
            cg.alpha = 1f;
            cg.blocksRaycasts = true;
            cg.interactable = true;
        }

        // start fully transparent
        foreach (var img in images)
        {
            var c = img.color;
            c.a = 0f;
            img.color = c;
        }

        float t = 0;

        while (t < duration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Clamp01(t / duration);

            foreach (var img in images)
            {
                var c = img.color;
                c.a = alpha;
                img.color = c;
            }

            yield return null;
        }
    }
    public void TurnOff()
    {
        gameObject.SetActive(false);
        hasfaded = false; // allow fading again
    }
    public bool checkForReveal(float x, float y)
    {


        if (x < posX - widthHalf || x > posX + widthHalf || y < posY - heightHalf || y > posY + heightHalf)
        {
            return false;
        }
        return true;
    }

}
