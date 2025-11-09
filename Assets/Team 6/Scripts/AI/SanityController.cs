
// Chris: Should this script exist? I didn't see it on Team#2 branch so maybe Git messed up something with changes?

/*using System.Collections;
using UnityEngine;

namespace Core.AI.Enemies
{
    /// <summary>
    /// Applies fear over time to sheep detected by a DetectSheep component.
    /// Attach this to any enemy (Drekavac, Wolf, etc.) that has a DetectSheep child.
    /// </summary>
    [RequireComponent(typeof(DetectSheep))]
    public sealed class SanityController : MonoBehaviour
    {
        [Header("Fear Tick Settings")]
        [SerializeField] private float tickInterval = 0.5f;
        [SerializeField] private float fearPerTick = 1f;

        private DetectSheep _detector;
        private Coroutine _loop;
        private int _tickCounter;

        private void Awake()
        {
            _detector = GetComponent<DetectSheep>();
            if (_detector == null)
                Debug.LogError($"[SanityController] ❌ Missing DetectSheep component on {name}");
        }

        private void OnEnable()
        {
            Debug.Log($"[SanityController] ✅ Enabled on {name} (interval={tickInterval}, fear={fearPerTick})");
            _loop = StartCoroutine(FearTickLoop());
        }

        private void OnDisable()
        {
            if (_loop != null)
            {
                StopCoroutine(_loop);
                _loop = null;
                Debug.Log($"[SanityController] 🛑 Disabled on {name}");
            }
        }

        private IEnumerator FearTickLoop()
        {
            WaitForSeconds wait = new WaitForSeconds(tickInterval);

            while (true)
            {
                _tickCounter++;
                if (_detector == null)
                {
                    Debug.LogWarning($"[SanityController] ⚠️ No DetectSheep assigned for {name}, stopping loop.");
                    yield break;
                }

                var targets = _detector.visibleTargets;
                int count = targets.Count;

                Debug.Log($"[SanityController] Tick #{_tickCounter} | Detected {count} sheep");

                for (int i = 0; i < count; i++)
                {
                    Transform sheep = targets[i];
                    if (sheep == null) continue;

                    Debug.Log($"[SanityController] 🐑 {name} affecting {sheep.name}");

                    if (sheep.TryGetComponent<Core.AI.Sheep.SheepSanity>(out var sanity))
                    {
                        if (!sanity.IsPanicking) // ✅ skip already-panicking sheep
                        {
                            sanity.GainSanity(fearPerTick, transform.position);
                            Debug.Log($"[SanityController] {sheep.name} gained +{fearPerTick} sanity");
                        }
                        else
                        {
                            Debug.Log($"[SanityController] {sheep.name} already panicking, skipping tick.");
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"[SanityController] ⚠️ {sheep.name} has no SheepSanity component!");
                    }
                }

                yield return wait;
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (_detector == null) return;
            Gizmos.color = new Color(1f, 0.2f, 0.2f, 0.15f);
            Gizmos.DrawSphere(transform.position, _detector.radius);
            UnityEditor.Handles.color = Color.red;
            UnityEditor.Handles.Label(transform.position + Vector3.up * 2f, $"Fear radius = {_detector.radius}");
        }
#endif
    }
}
*/