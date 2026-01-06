using UnityEngine;

public class NEWInCameraDetector : MonoBehaviour
{
    public Camera cam;
    [SerializeField]
    private float maxViewDistance = 12f;
    [SerializeField, ReadOnly] 
    private bool _isVisible;

    public bool IsVisible => _isVisible; 

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

        Vector3 camPos = cam.transform.position;
        float maxDistSqr = maxViewDistance * maxViewDistance;

        foreach (var col in colliders)
        {
            if (col == null) continue;

            
            if ((col.bounds.center - camPos).sqrMagnitude > maxDistSqr)
                continue;

            if (GeometryUtility.TestPlanesAABB(camFrustum, col.bounds))
            {
                currentlyVisible = true;
                break;
            }
        }

        if (!currentlyVisible)
        {
            foreach (var rend in renderers)
            {
                if (rend == null) continue;

                if ((rend.bounds.center - camPos).sqrMagnitude > maxDistSqr)
                    continue;

                if (GeometryUtility.TestPlanesAABB(camFrustum, rend.bounds))
                {
                    currentlyVisible = true;
                    break;
                }
            }
        }

        _isVisible = currentlyVisible;
    }
}
