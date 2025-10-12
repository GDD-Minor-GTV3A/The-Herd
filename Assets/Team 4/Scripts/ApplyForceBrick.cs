    using UnityEngine;

public class ApplyForceBrick : MonoBehaviour
{
    Rigidbody rb;
    public bool isLeft = false;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (isLeft)
        {
            rb.AddForce(Vector3.left * 20, ForceMode.Impulse);
            return;
        }
        else
        {
            rb.AddForce(Vector3.right * 20, ForceMode.Impulse);
        }
    }
}
