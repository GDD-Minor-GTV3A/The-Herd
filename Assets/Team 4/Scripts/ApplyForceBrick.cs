    using UnityEngine;

public class ApplyForceBrick : MonoBehaviour
{
    Rigidbody rb;
    public bool isLeft = false;
    public WallBreakTrigger wallBreak;


    public void Update()
    {
        if (wallBreak.forceApplied)
        {
            ForceApplied();
            wallBreak.forceApplied = false;
        }
    }
    public void ForceApplied()
    {
        rb = GetComponent<Rigidbody>();
        if (isLeft)
        {   
            rb.AddRelativeForce(Vector3.left * 1000, ForceMode.Impulse);
        }
        else
        {
            rb.AddRelativeForce(Vector3.right * 1000, ForceMode.Impulse);
        }
    }
}
