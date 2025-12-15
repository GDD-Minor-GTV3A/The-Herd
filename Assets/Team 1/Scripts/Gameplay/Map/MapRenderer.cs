using UnityEngine;
using UnityEngine.UI;

public class MapRenderer : MonoBehaviour
{
    [Header("References")]
    public MapData mapData;           // Map state (radius)
    public Image inkLayer;            // UI image that uses a shader with a _RevealAmount property

    [Header("Settings")]
    public float maxRadius = 200f;    // Max size of the revealable map

    void OnEnable()
    {
        // Listen for reveal updates
        mapData.OnReveal += UpdateMap;
    }

    void OnDisable()
    {
        mapData.OnReveal -= UpdateMap;
    }

    /// <summary>
    /// Converts radius into 0–1 range and passes it to the map shader.
    /// </summary>
    void UpdateMap()
    {
        float fill = Mathf.Clamp01(mapData.revealedRadius / maxRadius);

        // Shader must contain a float parameter named "_RevealAmount"
        inkLayer.material.SetFloat("_RevealAmount", fill);
    }
}
