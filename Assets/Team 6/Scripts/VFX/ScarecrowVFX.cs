using System.Collections;
using UnityEngine;

public class ScarecrowVFX : MonoBehaviour
{
    [Header("Visuals")]
    public Renderer headRenderer;      
    public Color glowColor = new Color(1f, 0.35f, 0f);
    [Range(0f, 8f)] public float glowIntensity = 4f;

    [Header("Particle & Light")]
    public ParticleSystem burstParticles;       
    public Light pointLight;                    
    public float lightIntensity = 3f;
    public float lightRange = 6f;

    [Header("Timing")]
    public float vfxDuration = 2f;
    public float fadeDuration = 0.8f;

    Material _instanceMat;
    Color _originalEmissionColor;
    bool _isPlaying;

    void Awake()
    {
        if (headRenderer == null) headRenderer = GetComponent<Renderer>();
        if (headRenderer != null)
        {
            _instanceMat = headRenderer.material;
            _instanceMat.EnableKeyword("_EMISSION");
            _originalEmissionColor = _instanceMat.HasProperty("_EmissionColor")
                ? _instanceMat.GetColor("_EmissionColor")
                : Color.black;
        }

        if (pointLight != null)
        {
            pointLight.intensity = 0f;
            pointLight.range = lightRange;
        }
    }

    public void TriggerVFX()
    {
        if (_isPlaying) return;
        StartCoroutine(PlayVFX());
    }

    IEnumerator PlayVFX()
    {
        _isPlaying = true;

        if (burstParticles != null) burstParticles.Play();
        if (pointLight != null) pointLight.intensity = lightIntensity;

        if (_instanceMat != null && _instanceMat.HasProperty("_EmissionColor"))
        {
            Color emission = glowColor * glowIntensity;
            _instanceMat.SetColor("_EmissionColor", emission);
            DynamicGI.SetEmissive(headRenderer, emission);
        }

        yield return new WaitForSeconds(vfxDuration - fadeDuration);

        float t = 0f;
        Color start = _instanceMat.GetColor("_EmissionColor");
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float p = t / fadeDuration;

            if (_instanceMat != null)
            {
                Color c = Color.Lerp(start, _originalEmissionColor, p);
                _instanceMat.SetColor("_EmissionColor", c);
                DynamicGI.SetEmissive(headRenderer, c);
            }

            if (pointLight != null)
                pointLight.intensity = Mathf.Lerp(lightIntensity, 0f, p);

            yield return null;
        }

        if (pointLight != null) pointLight.intensity = 0f;
        if (_instanceMat != null)
            _instanceMat.SetColor("_EmissionColor", _originalEmissionColor);

        _isPlaying = false;
    }
}
