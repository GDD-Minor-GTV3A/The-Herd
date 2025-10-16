using UnityEngine;

public class NEWInCameraDetector : MonoBehaviour
{
    public Camera cam;

    [SerializeField, ReadOnly] // You’ll define the ReadOnly attribute below
    private bool _isVisible;

    public bool IsVisible => _isVisible; // Public getter

    private Plane[] camFrustum;
    private Collider[] colliders;
    private Renderer[] renderers;

    void Start()
    {
        if (cam == null)
        {
            Debug.LogError("Camera not assigned.");
            return;
        }

        colliders = GetComponentsInChildren<Collider>();
        renderers = GetComponentsInChildren<Renderer>();
    }

    void Update()
    {
        if (cam == null) return;

        camFrustum = GeometryUtility.CalculateFrustumPlanes(cam);
        bool currentlyVisible = false;

        foreach (var col in colliders)
        {
            if (col != null && GeometryUtility.TestPlanesAABB(camFrustum, col.bounds))
            {
                currentlyVisible = true;
                break;
            }
        }

        if (!currentlyVisible)
        {
            foreach (var rend in renderers)
            {
                if (rend != null && GeometryUtility.TestPlanesAABB(camFrustum, rend.bounds))
                {
                    currentlyVisible = true;
                    break;
                }
            }
        }

        _isVisible = currentlyVisible;
    }
}
