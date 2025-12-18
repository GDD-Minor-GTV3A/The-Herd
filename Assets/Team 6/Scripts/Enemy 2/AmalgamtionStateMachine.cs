using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class AmalgamationStateMachine : MonoBehaviour
{
    [Header("References")]
    public NavMeshAgent agent;
    public Transform player;
    public List<Transform> patrolNodes;   // All the nodes in your maze
    public AmalgamationVision vision;     // Vision component on this enemy

    [Header("Spawn / Intro State")]
    [Tooltip("If true, Amalgamation will use the spawn intro (follow player -> enter trigger -> then patrol).")]
    public bool useSpawnIntroState = true;

    [Header("Targeting (Sheep)")]
    [Tooltip("LayerMask used by attacks to find Sheep-layer colliders.")]
    public LayerMask sheepMask; // set this to the Sheep layer in the Inspector

    [Tooltip("BoxCollider (set as Trigger) that ends the spawn intro when the Amalgamation enters it.")]
    public Collider spawnIntroEndTrigger;

    [Tooltip("Speed while following the player during the spawn intro.")]
    public float spawnIntroSpeed = 4f;

    [Tooltip("How long to wait at the box before switching into the normal PATROL behaviour.")]
    public float spawnWaitAtBoxTime = 2f;

    // Set true when the enemy enters the trigger collider
    [System.NonSerialized] public bool spawnIntroTriggerHit = false;

    [Header("Patrol Settings")]
    [Range(0f, 1f)] public float interceptChance = 0.6f;  // Chance to try to intersect player
    public float nodeReachThreshold = 0.5f;               // How close is "reached" a node?

    [Header("Path / Player Settings")]
    public float pathPassRadius = 2f;         // For debugging how close paths pass near the player

    [Header("Speed Settings")]
    public float normalSpeed = 3.5f;
    public float fastSpeed = 6f;
    public float closeEnoughRadius = 6f;      // Used for patrol speed logic

    [Header("Optional: Periodic Re-Path (Patrol)")]
    public bool periodicallyChangeTarget = false;
    public float minChangeInterval = 3f;
    public float maxChangeInterval = 6f;

    [Header("Suspicious Turn Settings (Patrol)")]
    public bool suspiciousTurnsEnabled = true;      // enable the “paranoid 180° turn” behaviour
    public float minSuspiciousTurnInterval = 2f;    // minimum seconds between potential turns
    public float maxSuspiciousTurnInterval = 3f;    // maximum seconds between potential turns

    public float suspiciousMinDistance = 0f;        // only spin if player is at least this far
    public float suspiciousMaxDistance = 30f;       // and at most this far

    public bool suspiciousRequirePlayerLOS = true;  // still used internally by patrol turn logic
    public float suspiciousTurnDuration = 1.5f;     // time (seconds) for a 180° suspicious turn

    [Range(0f, 360f)]
    public float playerLookAngleForSuspicion = 120f; // for old suspicion logic (still there if you want)

    [Header("Chase / Aggro Settings")]
    public float chasePreferredDistance = 30f;    // target distance to keep from the player
    public float chaseMinDistance = 5f;           // don't get closer than this
    public float chaseMaxDistance = 80f;          // if player gets farther than this, lose aggro
    public float chaseLoseSightDelay = 0.75f;     // how long we can lose LOS before dropping aggro
    public float chaseRotationSpeed = 10f;        // how fast we rotate to look at the player
    public float chaseSustainSpeed = 6f;          // sustained chase speed once we've reached the ring
    public float chasePreferredArriveTolerance = 1f;

    [Header("ShootLine (Teo) Attack Distance")]
    public float shootLineMinDistance = 25f;   // start using ShootLine at/after this distance
    public float shootLineMaxDistance = 60f;   // optional; don't use if player is insanely far
    public Transform lineFirePoint;          // spawn origin
    public GameObject lineBulletPrefab;      // bullet prefab
    public int lineBulletsInLine = 8;        // number of bullets
    public float lineBulletSpacing = 1f;     // spread between bullets
    public float lineIndicatorLength = 15f;

    // When losing aggro, we want to go back to a patrol node near (but not too close to) the player
    public float chaseReengageMinNodeDistance = 15f;
    public float chaseReengageMaxNodeDistance = 40f;

    [Header("Chase Re-Aggro Settings")]
    public float chaseReaggroCooldown = 2f;       // seconds after losing aggro before we can aggro again

    [Header("Attack / Pounce Settings")]
    public bool attackEnabled = true;

    // Time between attacks WHILE IN CHASE (random between these)
    public float attackIntervalMin = 2f;
    public float attackIntervalMax = 3f;

    // How close we want to get to the player during the attack “lunge”
    public float attackApproachDistance = 5f;

    // Tolerance for reaching that distance (so it doesn't need to be exact)
    public float attackArriveThreshold = 0.75f;

    // Speed while lunging in the attack state
    public float attackSpeed = 8f;

    [Header("Slam Cone Settings")]
    public ConeTelegraph slamTelegraph;
    public float slamRange = 12f;                        // outer radius of the cone
    [Range(0f, 1f)]
    public float slamInnerRadiusFactor = 0.5f;           // inner = slamRange * this (the "first half")
    [Range(0f, 360f)]
    public float slamAngle = 90f;                        // cone angle
    public float slamTelegraphTime = 0.7f;               // how long telegraph shows before hitting
    public float slamInnerDamage = 80f;                  // damage in inner cone
    public float slamOuterDamage = 40f;                  // damage in outer cone

    [Header("Shoot area Attack Settings")]
    [Tooltip("Inner radius (always damage / kill) in world units.")]
    public float secondAttackInnerRadius = 5f;

    [Tooltip("Outer radius (50/50 zone) in world units.")]
    public float secondAttackOuterRadius = 10f;

    [Range(0f, 360f)]
    [Tooltip("Angle of the cone. Use 360 for full circles, or e.g. 120 for a front cone.")]
    public float secondAttackAngle = 360f;

    [Tooltip("How long he CHARGES while moving towards the player before firing.")]
    public float secondAttackChargeTime = 1.5f;

    [Tooltip("How long he STAYS FROZEN after firing.")]
    public float secondAttackFirePauseTime = 1.0f;

    [Tooltip("Movement speed while charging this attack.")]
    public float secondAttackChargeSpeed = 5f;

    [Header("Second Attack Damage")]
    public float secondAttackInnerDamage = 60f;
    public float secondAttackOuterDamage = 40f;

    [Range(0f, 1f)]
    [Tooltip("Chance to hit the PLAYER in the OUTER ring (0.5 = 50/50).")]
    public float secondAttackOuterPlayerHitChance = 0.5f;

    [Range(0f, 1f)]
    [Tooltip("Chance to KILL SHEEP in the OUTER ring (0.5 = 50/50).")]
    public float secondAttackOuterSheepKillChance = 0.5f;

    [Header("Chase LOS Layers")]
    public LayerMask chasePlayerMask;      // player layer(s) for chase LOS
    public LayerMask chaseObstacleMask;    // obstacle layer(s) for chase LOS

    [Header("Animation")]
    public AmalgamationAnimBridge anim;

    [Header("Debug")]
    public bool debugLogs = true;          // Toggle verbose logs on/off

    // Used by PatrolState to know where to start after a chase
    [HideInInspector] public Transform forcedFirstPatrolNode;

    // When did we last lose aggro? Used for re-aggro cooldown.
    [System.NonSerialized, HideInInspector]
    public float lastAggroLostTime = -999f;

    private IAmalgamationState currentState;
    private AmalgamationPatrolState patrolState;
    private AmalgamationChaseState chaseState;
    private AmalgamationAttackState attackState;
    private AmalgamationSpawnIntroState spawnIntroState;

    // Expose states to other states (so Chase / Attack can switch)
    public AmalgamationPatrolState PatrolState => patrolState;
    public AmalgamationChaseState ChaseState => chaseState;
    public AmalgamationAttackState AttackState => attackState;
    public AmalgamationSpawnIntroState SpawnIntroState => spawnIntroState;

    private void Awake()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        if (vision == null)
            vision = GetComponent<AmalgamationVision>();

        if (anim == null)
            anim = GetComponentInChildren<AmalgamationAnimBridge>(true);

        lastAggroLostTime = -999f;

        // Create states
        patrolState = new AmalgamationPatrolState(this, agent, player, patrolNodes);
        chaseState = new AmalgamationChaseState(this, agent, player, patrolNodes);
        attackState = new AmalgamationAttackState(this, agent, player);
        spawnIntroState = new AmalgamationSpawnIntroState(this, agent);
    }

    private void Start()
    {
        // If we want to use the spawn intro and we have the required references, start there
        if (useSpawnIntroState && spawnIntroEndTrigger != null && player != null)
        {
            if (debugLogs)
            {
                Debug.Log($"[Amalgamation {gameObject.name}] Starting in SPAWN INTRO state.");
            }
            SwitchState(spawnIntroState);
        }
        else
        {
            if (debugLogs)
            {
                Debug.Log($"[Amalgamation {gameObject.name}] Starting directly in PATROL " +
                          "(spawn intro disabled or missing player/intro trigger).");
            }
            SwitchState(patrolState);
        }
    }

    private void Update()
    {
        currentState?.Tick();

        // Automatic transition: PATROL -> CHASE when we see player in front cone
        if (vision != null && currentState == patrolState && vision.CanSeePlayer)
        {
            // If lastAggroLostTime < 0, it means we've NEVER lost aggro yet -> no cooldown
            bool canReaggro = (lastAggroLostTime < 0f) ||
                              (Time.time >= lastAggroLostTime + chaseReaggroCooldown);

            if (canReaggro)
            {
                if (debugLogs)
                    Debug.Log($"[Amalgamation {gameObject.name}] Switching to CHASE (saw player in FRONT).");

                SwitchState(chaseState);
            }
            else if (debugLogs)
            {
                float remaining = (lastAggroLostTime + chaseReaggroCooldown) - Time.time;
                Debug.Log($"[Amalgamation {gameObject.name}] Saw player but still in re-aggro cooldown ({remaining:F2}s left).");
            }
        }

        // CHASE -> PATROL is handled inside AmalgamationChaseState.
    }

    // NEW: detect when we enter the dragged intro trigger collider
    private void OnTriggerEnter(Collider other)
    {
        if (!useSpawnIntroState) return;
        if (spawnIntroEndTrigger == null) return;

        if (other == spawnIntroEndTrigger)
        {
            spawnIntroTriggerHit = true;

            if (debugLogs)
                Debug.Log($"[Amalgamation {gameObject.name}] Spawn intro trigger ENTERED: {other.name}");
        }
    }

    public void SwitchState(IAmalgamationState newState)
    {
        if (debugLogs)
        {
            string from = currentState != null ? currentState.GetType().Name : "NONE";
            string to = newState != null ? newState.GetType().Name : "NULL";
            Debug.Log($"[Amalgamation {gameObject.name}] SWITCH {from} -> {to}  (t={Time.time:F2})");
        }
        currentState?.Exit();
        currentState = newState;
        currentState?.Enter();
    }

    private void OnDrawGizmosSelected()
    {
        // GREEN: "close enough" radius around the player (patrol speed logic)
        if (player != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(player.position, closeEnoughRadius);
        }

        // RED: suspicious min/max distance around this enemy (patrol paranoia)
        if (suspiciousTurnsEnabled)
        {
            Gizmos.color = Color.red;

            if (suspiciousMinDistance > 0f)
            {
                Gizmos.DrawWireSphere(transform.position, suspiciousMinDistance);
            }

            if (suspiciousMaxDistance > suspiciousMinDistance)
            {
                Gizmos.DrawWireSphere(transform.position, suspiciousMaxDistance);
            }
        }
    }
}
