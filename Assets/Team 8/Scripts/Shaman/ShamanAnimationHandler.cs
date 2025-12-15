using UnityEngine;

public class ShamanAnimationHandler : MonoBehaviour
{
    public float patrolDistance = 5f;   // How far forward from start
    public float speed = 2f;            // Movement speed
    public float turnSpeed = 180f;      // Degrees per second during turn

    private Vector3 startPos;
    private Vector3 endPos;
    private bool movingForward = true;
    private bool isTurning = false;
    private float turnAngle = 0f;

    void Start()
    {
        startPos = transform.position;
        endPos = startPos + transform.forward * patrolDistance;
    }

    void Update()
    {
        if (!isTurning)
        {
            Move();
        }
        else
        {
            Turn();
        }
    }

    void Move()
    {
        Vector3 target = movingForward ? endPos : startPos;

        // Move toward target
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

        // Check if reached target
        if (Vector3.Distance(transform.position, target) < 0.01f)
        {
            isTurning = true;
            turnAngle = 0f; // start turn
        }
    }

    void Turn()
    {
        // Rotate around Y-axis
        float step = turnSpeed * Time.deltaTime;
        transform.Rotate(0f, step, 0f);
        turnAngle += step;

        // Finish turn after 360°
        if (turnAngle >= 180f)
        {
            isTurning = false;
            movingForward = !movingForward; // switch direction
        }
    }
}