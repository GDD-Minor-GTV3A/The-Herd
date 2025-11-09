// using UnityEngine;
// using Core.AI.Sheep;
// using System.Collections.Generic;

// [DisallowMultipleComponent]
// [RequireComponent(typeof(SheepStateManager))]
// public class SheepPanicController : MonoBehaviour
// {
//     [Header("Sanity Settings")]
//     [SerializeField] private int sanity = 0;
//     [SerializeField] private int maxSanity = 100;
//     [SerializeField] private int panicThreshold = 20;

//     [Header("Panic Behaviour")]
//     [SerializeField] private float panicDuration = 3f;
//     [SerializeField] private float panicSpeedMultiplier = 1.8f;
//     [SerializeField] private bool debugLogs = true;

//     private SheepStateManager stateManager;
//     private bool isPanicking = false;
//     private float panicTimer = 0f;
//     private float originalSpeed;
//     private bool herdDisabled = false;

//     private void Awake()
//     {
//         stateManager = GetComponent<SheepStateManager>();
//         if (stateManager?.Agent != null)
//             originalSpeed = stateManager.Agent.speed;
//     }

//     private void Update()
//     {
//         if (!isPanicking) return;

//         panicTimer -= Time.deltaTime;
//         if (panicTimer <= 0f)
//         {
//             ResetSanityAfterPanic();
//         }
//     }

//     public void GainSanity(int amount)
//     {
//         if (isPanicking) return;

//         sanity = Mathf.Clamp(sanity + amount, 0, maxSanity);
//         if (debugLogs) Debug.Log($"[{name}] sanity = {sanity}");

//         if (sanity >= panicThreshold)
//             EnterPanicState();
//     }

//     private void EnterPanicState()
//     {
//         if (isPanicking) return;

//         Debug.Log($"[{name}] 🧠 PANIC TRIGGERED — sanity {sanity}");
//         isPanicking = true;
//         panicTimer = panicDuration;

//         // Lock the herd state system so it can't override panic
//         stateManager.LockStateFromExternal();
//         if (debugLogs) Debug.Log($"[{name}] 🔒 Locked state for panic.");

//         // Disable herd participation (stop grazing updates)
//         if (!herdDisabled)
//         {
//             stateManager.DisableHerdParticipation();
//             herdDisabled = true;
//             if (debugLogs) Debug.Log($"[{name}] 🚫 Herd participation disabled during panic.");
//         }

//         // Clear neighbours so it doesn't flock during panic
//         stateManager.SetNeighbours(new List<Transform>());

//         // Increase speed and start panic state
//         if (stateManager.Agent != null)
//         {
//             originalSpeed = stateManager.Agent.speed;
//             stateManager.Agent.speed = originalSpeed * panicSpeedMultiplier;
//             stateManager.Agent.isStopped = false;
//         }

//         // Force panic behaviour
//         stateManager.SetState<SheepPanicState>();
//         stateManager.Animation?.SetState((int)SheepAnimState.Run);
//     }

//     private void ResetSanityAfterPanic()
//     {
//         if (!isPanicking) return;

//         Debug.Log($"[{name}] 🧘 RESET SANITY AFTER PANIC");
//         isPanicking = false;
//         sanity = 0;

//         var currentType = stateManager.GetCurrentStateType();
//         if (currentType == typeof(SheepPanicState))
//         {
//             Debug.Log($"[{name}] 🔄 Forcing stop on PanicState before transition.");
//             stateManager.StopCurrentState();
//         }

//         if (stateManager.Agent != null)
//         {
//             stateManager.Agent.isStopped = true;
//             stateManager.Agent.ResetPath();
//         }

//         if (herdDisabled)
//         {
//             stateManager.EnableHerdParticipation();
//             herdDisabled = false;
//             Debug.Log($"[{name}] ✅ Herd participation re-enabled.");
//         }

//         // --- 🔧 key addition ---
//         // Immediately force one herd logic tick so the sheep gets a fresh move target
//         if (debugLogs) Debug.Log($"[{name}] 🔁 Forcing herd re-sync tick after panic.");
//         stateManager.ForceHerdResync();
//         // --- end addition ---

//         stateManager.UnlockStateFromExternal();
//         Debug.Log($"[{name}] 🔓 State unlocked after panic.");

//         stateManager.SetState<SheepFollowState>();

//         if (stateManager.Agent != null)
//         {
//             stateManager.Agent.isStopped = false;
//             stateManager.Agent.speed = originalSpeed;
//         }

//         Debug.Log($"[{name}] ✅ Panic ended — sanity reset, returning to herd.");
//     }


//     public int CurrentSanity => sanity;
//     public bool IsPanicking => isPanicking;
// }
