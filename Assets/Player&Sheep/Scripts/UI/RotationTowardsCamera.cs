using UnityEngine;

/// <summary>
/// Rotation object towards camera without y axis. Mainly used for UI.
/// </summary>
public class RotationTowardsCamera : MonoBehaviour
{
    private Camera _mainCamera;


    private void Start()
    {
        _mainCamera = Camera.main;
    }


    private void Update()
    {
        Vector3 direction = _mainCamera.transform.position - transform.position;
        direction.y = 1;
        direction.Normalize();
        transform.forward = direction;
    }
}