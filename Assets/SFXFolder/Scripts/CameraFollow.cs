using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // assign the Player here in Inspector
    public Vector3 offset = new Vector3(0, 5, -8);
    public float smoothSpeed = 10f;

    void LateUpdate()
    {
        if (target == null) return;

        // keep camera locked at player + offset
        Vector3 desiredPosition = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
    }
}