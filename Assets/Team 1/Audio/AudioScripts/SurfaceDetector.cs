using UnityEngine;

public class SurfaceDetector : MonoBehaviour
{
    public string CurrentSurface { get; private set; }

    private void Update()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 2f))
        {
            CurrentSurface = hit.collider.tag;
        }
        else
        {
            CurrentSurface = "Default";
        }
    }
}
