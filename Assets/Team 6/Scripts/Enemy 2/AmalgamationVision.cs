using UnityEngine;

public class AmalgamationVision : MonoBehaviour
{
    [Header("References")]
    public Transform player;

    [Header("FRONT Vision Settings")]
    public float viewRadius = 12f;
    [Range(0f, 360f)] public float viewAngle = 120f;

    [Header("BACK Vision Settings")]
    public bool useBackVision = true;
    public float backViewRadius = 8f;
    [Range(0f, 360f)] public float backViewAngle = 90f;

    [Header("Heights")]
    public float enemyEyeHeight = 1.6f;     // height of Amalgamation's "eyes"
    public float playerEyeHeight = 1.6f;    // height of player's "eyes"

    [Header("Layers")]
    public LayerMask playerMask;            // layer(s) the player is on
    public LayerMask obstacleMask;          // walls, props, etc.

    [Header("Check Settings")]
    public float checkInterval = 0.1f;      // how often to update vision (seconds)

    [Header("Debug")]
    public bool debugLogs = false;

    /// <summary>
    /// True if Amalgamation currently has line-of-sight to the player in FRONT cone.
    /// </summary>
    public bool CanSeePlayer { get; private set; }

    /// <summary>
    /// True if Amalgamation currently has line-of-sight to the player in BACK cone.
    /// </summary>
    public bool CanSeePlayerBack { get; private set; }

    private float lastCheckTime;

    private void Reset()
    {
        // Auto-find player by tag if possible
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null)
            player = p.transform;
    }

    private void Update()
    {
        if (Time.time >= lastCheckTime + checkInterval)
        {
            lastCheckTime = Time.time;
            UpdateVision();
        }
    }

    private void UpdateVision()
    {
        if (player == null)
        {
            SetFrontSeen(false, "No player reference.");
            SetBackSeen(false, "No player reference.");
            return;
        }

        // Eye origins (so ray goes head -> head)
        Vector3 enemyEye = transform.position + Vector3.up * enemyEyeHeight;
        Vector3 playerEye = player.position + Vector3.up * playerEyeHeight;

        Vector3 toPlayer = playerEye - enemyEye;
        float distToPlayer = toPlayer.magnitude;

        if (distToPlayer <= 0.001f)
        {
            // Practically on top of us; just treat direction as forward
            toPlayer = transform.forward;
            distToPlayer = 0.001f;
        }

        Vector3 dirToPlayer = toPlayer / distToPlayer;
        float rayDist = distToPlayer + 0.1f;
        int mask = obstacleMask | playerMask;

        bool newFrontSeen = false;
        bool newBackSeen = false;
        string frontReason = "";
        string backReason = "";

        // ========================= FRONT VISION =========================
        if (distToPlayer > viewRadius)
        {
            newFrontSeen = false;
            frontReason = "Player out of FRONT viewRadius.";
            Debug.DrawLine(enemyEye, playerEye, Color.gray, checkInterval);
        }
        else
        {
            float angleToPlayerFront = Vector3.Angle(transform.forward, dirToPlayer);
            if (angleToPlayerFront > viewAngle * 0.5f)
            {
                newFrontSeen = false;
                frontReason = "Player outside FRONT viewAngle.";
                Debug.DrawLine(enemyEye, playerEye, Color.gray, checkInterval);
            }
            else
            {
                // Inside radius & angle – check LOS
                if (Physics.Raycast(enemyEye, dirToPlayer, out RaycastHit hitFront, rayDist, mask))
                {
                    bool hitIsPlayer = ((1 << hitFront.collider.gameObject.layer) & playerMask) != 0;

                    if (hitIsPlayer)
                    {
                        newFrontSeen = true;
                        frontReason = $"Player seen in FRONT. Dist={distToPlayer:F2}, Angle={angleToPlayerFront:F2}";
                        Debug.DrawLine(enemyEye, playerEye, Color.red, checkInterval);
                    }
                    else
                    {
                        newFrontSeen = false;
                        frontReason = $"FRONT vision blocked by '{hitFront.collider.name}'.";
                        Debug.DrawLine(enemyEye, hitFront.point, Color.gray, checkInterval);
                    }
                }
                else
                {
                    newFrontSeen = false;
                    frontReason = "FRONT raycast hit nothing.";
                    Debug.DrawLine(enemyEye, playerEye, Color.gray, checkInterval);
                }
            }
        }

        // ========================= BACK VISION ==========================
        if (useBackVision)
        {
            if (distToPlayer > backViewRadius)
            {
                newBackSeen = false;
                backReason = "Player out of BACK viewRadius.";
            }
            else
            {
                // Back cone is centered behind us: -transform.forward
                float angleToPlayerBack = Vector3.Angle(-transform.forward, dirToPlayer);
                if (angleToPlayerBack > backViewAngle * 0.5f)
                {
                    newBackSeen = false;
                    backReason = "Player outside BACK viewAngle.";
                }
                else
                {
                    // Inside back radius & angle – check LOS with same ray
                    if (Physics.Raycast(enemyEye, dirToPlayer, out RaycastHit hitBack, rayDist, mask))
                    {
                        bool hitIsPlayerBack = ((1 << hitBack.collider.gameObject.layer) & playerMask) != 0;

                        if (hitIsPlayerBack)
                        {
                            newBackSeen = true;
                            backReason = $"Player seen in BACK. Dist={distToPlayer:F2}, AngleBack={angleToPlayerBack:F2}";
                            Debug.DrawLine(enemyEye, playerEye, Color.magenta, checkInterval);
                        }
                        else
                        {
                            newBackSeen = false;
                            backReason = $"BACK vision blocked by '{hitBack.collider.name}'.";
                            Debug.DrawLine(enemyEye, hitBack.point, Color.gray, checkInterval);
                        }
                    }
                    else
                    {
                        newBackSeen = false;
                        backReason = "BACK raycast hit nothing.";
                        Debug.DrawLine(enemyEye, playerEye, Color.gray, checkInterval);
                    }
                }
            }
        }
        else
        {
            newBackSeen = false;
            backReason = "Back vision disabled.";
        }

        SetFrontSeen(newFrontSeen, frontReason);
        SetBackSeen(newBackSeen, backReason);
    }

    private void SetFrontSeen(bool seen, string reason)
    {
        if (CanSeePlayer == seen) return;

        CanSeePlayer = seen;
        if (debugLogs)
        {
            Debug.Log($"[AmalgamationVision {gameObject.name}] FRONT CanSeePlayer = {seen}. Reason: {reason}");
        }
    }

    private void SetBackSeen(bool seen, string reason)
    {
        if (CanSeePlayerBack == seen) return;

        CanSeePlayerBack = seen;
        if (debugLogs)
        {
            Debug.Log($"[AmalgamationVision {gameObject.name}] BACK CanSeePlayerBack = {seen}. Reason: {reason}");
        }
    }

    #region Gizmos

    private void OnDrawGizmosSelected()
    {
        Vector3 enemyEye = transform.position + Vector3.up * enemyEyeHeight;

        // FRONT radius
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(enemyEye, viewRadius);

        // FRONT FOV cone edges
        Gizmos.color = Color.cyan;
        Vector3 frontLeft = DirFromAngle(-viewAngle / 2f, false);
        Vector3 frontRight = DirFromAngle(viewAngle / 2f, false);
        Gizmos.DrawLine(enemyEye, enemyEye + frontLeft * viewRadius);
        Gizmos.DrawLine(enemyEye, enemyEye + frontRight * viewRadius);

        // BACK radius + cone
        if (useBackVision)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(enemyEye, backViewRadius);

            // Back cone is centered at 180 degrees relative to forward
            Vector3 backLeft = DirFromAngle(180f - backViewAngle / 2f, false);
            Vector3 backRight = DirFromAngle(180f + backViewAngle / 2f, false);
            Gizmos.DrawLine(enemyEye, enemyEye + backLeft * backViewRadius);
            Gizmos.DrawLine(enemyEye, enemyEye + backRight * backViewRadius);
        }

        // If we can currently see the player in FRONT, draw a red gizmo line to their head
        if (Application.isPlaying && CanSeePlayer && player != null)
        {
            Gizmos.color = Color.red;
            Vector3 playerEye = player.position + Vector3.up * playerEyeHeight;
            Gizmos.DrawLine(enemyEye, playerEye);
        }

        // If we can currently see the player in BACK, draw a magenta gizmo line
        if (Application.isPlaying && CanSeePlayerBack && player != null)
        {
            Gizmos.color = Color.magenta;
            Vector3 playerEye = player.position + Vector3.up * playerEyeHeight;
            Gizmos.DrawLine(enemyEye, playerEye);
        }
    }

    /// <summary>
    /// Returns a direction vector from an angle (in degrees).
    /// If global == false, the angle is relative to the Amalgamation's current Y rotation.
    /// </summary>
    public Vector3 DirFromAngle(float angleDegrees, bool global)
    {
        if (!global)
        {
            angleDegrees += transform.eulerAngles.y;
        }
        float rad = angleDegrees * Mathf.Deg2Rad;
        return new Vector3(Mathf.Sin(rad), 0f, Mathf.Cos(rad));
    }

    #endregion
}
