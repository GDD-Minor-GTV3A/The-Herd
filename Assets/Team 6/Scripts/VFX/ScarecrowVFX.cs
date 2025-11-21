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
   [SerializeField] private bool _isPlaying = false;
    int _emissionColorID;

    void Start()
    {
        if (headRenderer == null) headRenderer = GetComponent<Renderer>();

        if (headRenderer != null)
        {
            _instanceMat = headRenderer.material;
            _emissionColorID = Shader.PropertyToID("_EmissionColor");

            if (_instanceMat.HasProperty(_emissionColorID))
                _originalEmissionColor = _instanceMat.GetColor(_emissionColorID);
            else
                _originalEmissionColor = Color.black;

            _instanceMat.EnableKeyword("_EMISSION");
        }

        if (pointLight != null)
        {
            pointLight.intensity = 0f;
            pointLight.range = lightRange;
        }
    }



  void Update()
    {
        if (_isPlaying = true) {
            StartCoroutine(PlayVFX());
            Debug.Log("StartedVFX");
        }
      
    }

    IEnumerator PlayVFX()
    {
        Debug.Log("IsPlaying");

        if (burstParticles != null) burstParticles.Play();
        if (pointLight != null) pointLight.intensity = lightIntensity;

        if (_instanceMat != null && _instanceMat.HasProperty(_emissionColorID))
        {
            Color emission = glowColor * Mathf.LinearToGammaSpace(glowIntensity);
            _instanceMat.SetColor(_emissionColorID, emission);
        }

        yield return new WaitForSeconds(vfxDuration - fadeDuration);

        float t = 0f;
        Color start = _instanceMat.GetColor(_emissionColorID);
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float p = t / fadeDuration;

            if (_instanceMat != null)
            {
                Color c = Color.Lerp(start, _originalEmissionColor, p);
                _instanceMat.SetColor(_emissionColorID, c);
            }

            if (pointLight != null)
                pointLight.intensity = Mathf.Lerp(lightIntensity, 0f, p);

            yield return null;
        }

        if (pointLight != null) pointLight.intensity = 0f;
        if (_instanceMat != null)
            _instanceMat.SetColor(_emissionColorID, _originalEmissionColor);

        _isPlaying = false;
    }
}
