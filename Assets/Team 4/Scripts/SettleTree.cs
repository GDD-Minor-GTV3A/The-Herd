using UnityEngine;

public class SettleTree : MonoBehaviour
{
    private Rigidbody rb;

    private void Awake() //Fixed compile error caused by initialized function (Yeremey from Integration)
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("SettleTree: Missing Rigidbody!", this);
            enabled = false;
        }
    }

    private void Update()
    {
        if (rb == null) return;

        if (rb.IsSleeping())
        {
            rb.isKinematic = true;
            enabled = false;
        }
    }
}
