using UnityEngine;

public class CandleFlicker : MonoBehaviour
{
    public Light candleLight;
    public float minIntensity = 0.8f;
    public float maxIntensity = 1.5f;
    public float flickerSpeed = 2f;

    void Update()
    {
        if (candleLight != null)
        {
            candleLight.intensity = Mathf.Lerp(
                candleLight.intensity,
                Random.Range(minIntensity, maxIntensity),
                Time.deltaTime * flickerSpeed
            );
        }
    }
}
