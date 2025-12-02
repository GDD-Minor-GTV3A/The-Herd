using System.Collections;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    [Header("Detection Settings")]
    public float radius = 10f;
    [Range(0, 360)] public float angle = 90f;

    [Header("References")]
    public GameObject playerRef;

    [Header("Obstruction Layers")]
    public LayerMask obstructionMask; // walls, props, etc.

    [Header("Debug")]
    public bool canSeePlayer;

    // Internal
    private WaitForSeconds _wait = new WaitForSeconds(0.05f); // check 10x per sec

    private void Start()
    {
        // Find the player automatically if not assigned
        if (playerRef == null)
        {
            playerRef = GameObject.FindGameObjectWithTag("Player");
            if (playerRef == null)
            {
                Debug.LogError("[FieldOfView] No GameObject with tag 'Player' found!");
                enabled = false;
                return;
            }
        }

        StartCoroutine(FOVRoutine());
    }

    private IEnumerator FOVRoutine()
    {
        while (true)
        {
            yield return _wait;
            FieldOfViewCheck();
        }
    }

    private void FieldOfViewCheck()
    {
        canSeePlayer = false;

        if (playerRef == null) return;

        Vector3 eyePos = transform.position + Vector3.up * 1.5f;
        Vector3 directionToTarget = (playerRef.transform.position - eyePos).normalized;
        float distanceToTarget = Vector3.Distance(eyePos, playerRef.transform.position);

        // Check if player is within radius
        if (distanceToTarget <= radius)
        {
            // Check if player is within angle
            if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
            {
                // Raycast to ensure nothing blocks line of sight
                if (!Physics.Raycast(eyePos, directionToTarget, distanceToTarget, obstructionMask))
                {
                    canSeePlayer = true;
                }
            }
        }
    }

    // === Debug Gizmos ===
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);

        Vector3 angle01 = DirectionFromAngle(-angle / 2, false);
        Vector3 angle02 = DirectionFromAngle(angle / 2, false);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + angle01 * radius);
        Gizmos.DrawLine(transform.position, transform.position + angle02 * radius);

        if (canSeePlayer && playerRef != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position + Vector3.up * 1.5f, playerRef.transform.position);
        }
    }

    private Vector3 DirectionFromAngle(float angleInDegrees, bool global)
    {
        if (!global)
            angleInDegrees += transform.eulerAngles.y;
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}
