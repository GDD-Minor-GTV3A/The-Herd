using UnityEngine;

public class SettleTree : MonoBehaviour
{
    private Rigidbody rb;

    public void Initialize()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Update()
    {
        if (rb.IsSleeping())
        {
            rb.isKinematic = true;

            this.enabled = false;
        }
    }
}