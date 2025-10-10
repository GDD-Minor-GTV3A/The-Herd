using UnityEngine;

public class CameraFollowing : MonoBehaviour
{
    [SerializeField] private Transform target;

    void Update()
    {
        transform.position = target.GetChild(0).position + new Vector3(-20.3f, 10.3f, -10.8f);
    }
}

// player
// Vector3(71.9899979,24.6000004,275.230011)

// cam
// Vector3(53.6199989,34.9599991,264.420013)
