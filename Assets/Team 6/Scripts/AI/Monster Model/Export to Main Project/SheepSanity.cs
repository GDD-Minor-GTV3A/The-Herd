using UnityEngine;

public class SheepSanity : MonoBehaviour
{
    public int maxSanity = 100;
    public int currentSanity = 0;
    public float flashDuration = 0.2f;

    private Renderer rend;
    private Color baseColor;
    private Color blueColor = Color.blue;
    private bool isBlue = false;

    private void Awake()
    {
        currentSanity = 0;
        rend = GetComponentInChildren<Renderer>();

        if (rend != null)
        {
            baseColor = rend.material.color;
        }
        else
        {
            Debug.LogWarning($"{gameObject.name} has no Renderer for color effects.");
        }
    }

    public void GainSanity(int amount)
    {
        if (currentSanity >= maxSanity)
            return;

        currentSanity += amount;
        currentSanity = Mathf.Min(currentSanity, maxSanity);
        Debug.Log($"{gameObject.name} gained {amount} sanity. Current: {currentSanity}");

        if (rend != null)
        {
            StopAllCoroutines();
            StartCoroutine(FlashRedThenCheckState());
        }
    }

    private System.Collections.IEnumerator FlashRedThenCheckState()
    {
        rend.material.color = Color.red;
        yield return new WaitForSeconds(flashDuration);

        if (currentSanity >= 20)
        {
            rend.material.color = blueColor;
            isBlue = true;
        }
        else
        {
            rend.material.color = baseColor;
            isBlue = false;
        }
    }
}
