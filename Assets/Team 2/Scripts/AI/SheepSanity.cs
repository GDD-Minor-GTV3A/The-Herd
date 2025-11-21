

// Chris: Should this script exist? I didn't see it on Team#2 branch so maybe Git messed up something with changes?

/*using UnityEngine;
using Core.AI.Sheep.Personality;

namespace Core.AI.Sheep
{
    [RequireComponent(typeof(SheepStateManager))]
    public sealed class SheepSanity : MonoBehaviour
    {
        [Header("Sanity")]
        [SerializeField] private int maxSanity = 100;
        [SerializeField] private int panicThresholdMin = 5;
        [SerializeField] private int panicThresholdMax = 30;

        [Header("Ripple (herd contagion)")]
        [SerializeField] private float rippleRadius = 8f;
        [SerializeField] private int rippleAmount = 3;
        [SerializeField] private LayerMask sheepMask;

        private SheepStateManager _sm;

        [SerializeField, Tooltip("Current sanity level - increases with fear, triggers panic at threshold")]
        private int _current;
        [SerializeField, Tooltip("Random threshold between min/max where this sheep will panic")]
        private int _panicThreshold;
        private bool _isPanicking;

        public PersonalityBehaviorContext BehaviorContext { get; private set; }

        public bool IsPanicking => _isPanicking;

        private void Awake()
        {
            _sm = GetComponent<SheepStateManager>();
            _panicThreshold = Random.Range(panicThresholdMin, panicThresholdMax + 1);
            BehaviorContext = new PersonalityBehaviorContext();
        }

        private void Update()
        {
#if UNITY_EDITOR
            Vector3 screen = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 1.5f);
            if (screen.z > 0)
                GUI.Label(new Rect(screen.x, Screen.height - screen.y, 120, 20), $"{name} S:{_current}");
#endif
        }

        public void GainSanity(float amount, Vector3 threatPosition)
        {
            if (_isPanicking) return;

            int before = _current;
            _current = Mathf.Clamp(_current + Mathf.RoundToInt(amount), 0, maxSanity);
            Debug.Log($"[{name}] 🧠 GainSanity({amount}) | before={before} → after={_current}/{maxSanity}");

            _sm.BehaviorContext.ThreatPosition = threatPosition;
            _sm.BehaviorContext.HasThreat = true;

            if (_current >= _panicThreshold && !_isPanicking)
            {
                Debug.Log($"[{name}] 🧠 PANIC TRIGGERED! Threshold={_panicThreshold}");
                _isPanicking = true;

                PanicRipple(threatPosition);
                _sm.SetState<SheepPanicState>();
            }
        }

        public void CalmDown()
        {
            _isPanicking = false;
            _current = 0;
            _sm.BehaviorContext.HasThreat = false;
        }

        private void PanicRipple(Vector3 source)
        {
            if (rippleRadius <= 0f || rippleAmount <= 0) return;

            var hits = Physics.OverlapSphere(transform.position, rippleRadius, sheepMask, QueryTriggerInteraction.Ignore);
            foreach (var h in hits)
            {
                if (h.transform == transform) continue;
                if (h.TryGetComponent<SheepSanity>(out var other))
                {
                    other.GainSanity(rippleAmount, source);
                }
            }
        }

        public void ForceReset()
        {
            Debug.Log($"[{name}] ForceReset() — resetting sanity to 0 and clearing panic state.");
            _current = 0;
            _isPanicking = false;
            _sm.BehaviorContext.HasThreat = false;
            _sm.BehaviorContext.ThreatPosition = Vector3.zero;
        }
    }
}
*/