    using UnityEngine;

public class ApplyForceBrick : MonoBehaviour
{
    Rigidbody rb;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.AddForce(Vector3.right * 15, ForceMode.Impulse);
    }
}
