using UnityEngine;

public class FallingWallPhysics : MonoBehaviour
{
    public WallBreakTrigger wallBreak;
    Rigidbody rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;

    }

    // Update is called once per frame
    void Update()
    {
        if (wallBreak.forceApplied)
        {
            rb.isKinematic = false;
        }
    }
}
