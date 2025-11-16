using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeInOnEnable : MonoBehaviour
{
    public float duration = 1f;   // Fade speed
    private Image img;

    void Awake()
    {
        img = GetComponent<Image>();
    }

    void OnEnable()
    {
        StartCoroutine(Fade());
    }

    /// <summary>
    /// Smooth fade-in effect for UI Image.
    /// </summary>
    IEnumerator Fade()
    {
        float t = 0f;
        Color c = img.color;

        // Start fully transparent
        c.a = 0;
        img.color = c;

        // Fade to fully visible over duration
        while (t < duration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(0, 1, t / duration);
            img.color = c;

            yield return null;
        }
    }
}
