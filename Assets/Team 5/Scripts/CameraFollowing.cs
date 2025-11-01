using UnityEngine;

public class CameraFollowing : MonoBehaviour
{
    [SerializeField] private Transform target;

    void Update()
    {
        transform.position = target.GetChild(0).position + new Vector3(-25f, 30f, -25f);
    }
}
