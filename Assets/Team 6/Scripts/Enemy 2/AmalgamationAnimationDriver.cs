using UnityEngine;
using UnityEngine.AI;

[DefaultExecutionOrder(100)]               // run after most scripts
[RequireComponent(typeof(NavMeshAgent))]
public class AmalgamationAnimationDriverDebug : MonoBehaviour
{
    [Header("References (assign the EXACT animator playing your controller)")]
    public Animator animator;              // drag your Model/Armature Animator here
    public NavMeshAgent agent;             // auto-filled if left empty

    [Header("Tuning")]
    public float maxRunSpeed = 6f;         // set to your fastest agent speed
    public float dampTime = 0.1f;          // animator damping

    [Header("Debug")]
    public bool printEveryQuarterSecond = true;
    public bool drawHud = true;
    public float logInterval = 0.25f;

    static readonly int SpeedHash = Animator.StringToHash("Speed");
    float lastPrint;

    void Awake()
    {
        if (!agent) agent = GetComponent<NavMeshAgent>();
        if (!animator) animator = GetComponentInChildren<Animator>(true);

        if (!animator)
            Debug.LogError($"[AnimDbg] No Animator found under '{name}'. Drag the Armature/Model Animator here.");

        // Helpful once at start
        if (animator)
        {
            Debug.Log($"[AnimDbg] Using animator '{animator.gameObject.name}', " +
                      $"culling={animator.cullingMode}, rootMotion={animator.applyRootMotion}");
        }
    }

    void Update()
    {
        if (!agent || !animator) return;

        // Use desiredVelocity to be responsive even when agent is planning/braking.
        float desired = agent.desiredVelocity.magnitude;
        float actual  = agent.velocity.magnitude;
        float current = Mathf.Max(desired, actual);

        float speed01 = (maxRunSpeed > 0.001f) ? Mathf.Clamp01(current / maxRunSpeed) : 0f;
        animator.SetFloat(SpeedHash, speed01, dampTime, Time.deltaTime);

        if (printEveryQuarterSecond && (Time.time - lastPrint) >= logInterval)
        {
            lastPrint = Time.time;

            // Query current state name safely
            string stateName = GetStateName(animator);
            string clipName  = GetClipName(animator);

            Debug.Log(
                $"[AnimDbg '{name}'] state={stateName} clip={clipName} " +
                $"SpeedParam={animator.GetFloat(SpeedHash):0.00} " +
                $"desired={desired:0.00} actual={actual:0.00} " +
                $"hasPath={agent.hasPath} pending={agent.pathPending} rem={agent.remainingDistance:0.00} " +
                $"stopped={agent.isStopped} updPos={agent.updatePosition} updRot={agent.updateRotation} " +
                $"cull={animator.cullingMode} enabled={animator.enabled}"
            );
        }
    }

    string GetStateName(Animator anim)
    {
        // Avoid allocations/complex checks: just see if it matches your two states.
        var st = anim.GetCurrentAnimatorStateInfo(0);
        if (st.IsName("Idle"))   return "Idle";
        if (st.IsName("Moving")) return "Moving";
        // fallback: return hash
        return st.shortNameHash.ToString();
    }

    string GetClipName(Animator anim)
    {
        // Safe: if no clip info, return "(none)"
        var infos = anim.GetCurrentAnimatorClipInfo(0);
        if (infos != null && infos.Length > 0 && infos[0].clip != null)
            return infos[0].clip.name;
        return "(none)";
    }

    void OnGUI()
    {
        if (!drawHud || !Application.isPlaying || !agent || !animator) return;

        var stName  = GetStateName(animator);
        var clip    = GetClipName(animator);
        float spd   = animator.GetFloat(SpeedHash);
        float d     = agent.desiredVelocity.magnitude;
        float a     = agent.velocity.magnitude;

        string text =
            $"ANIM DEBUG — {name}\n" +
            $"State: {stName}   Clip: {clip}\n" +
            $"Speed param: {spd:0.00}\n" +
            $"Agent desired/actual: {d:0.00} / {a:0.00}\n" +
            $"hasPath:{agent.hasPath}  pending:{agent.pathPending}  rem:{agent.remainingDistance:0.00}\n" +
            $"stopped:{agent.isStopped}  updPos:{agent.updatePosition}  updRot:{agent.updateRotation}\n" +
            $"culling:{animator.cullingMode}  animatorEnabled:{animator.enabled}";

        var rect = new Rect(12, 12, 520, 140);
        var bg = new Color(0, 0, 0, 0.55f);
        var old = GUI.color;
        GUI.color = bg; GUI.Box(rect, GUIContent.none);
        GUI.color = Color.white; GUI.Label(rect, text);
        GUI.color = old;
    }
}
