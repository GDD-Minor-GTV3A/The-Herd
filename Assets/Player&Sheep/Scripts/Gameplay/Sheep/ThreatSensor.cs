using UnityEngine;
using System.Collections.Generic;

namespace Core.AI.Sheep
{
    [DisallowMultipleComponent]
    public sealed class ThreatSensor : MonoBehaviour
    {
        [SerializeField] private SheepStateManager _sheep;
        [SerializeField] private float _scanRadius = 20f;
        [SerializeField] private float _refresh = 0.25f;
        [SerializeField] private LayerMask enemyMask;

        private float _next;

        private readonly Dictionary<Transform, float> _seen = new();

        private void Reset()
        {
            _sheep = GetComponent<SheepStateManager>();
        }

        private void Update()
        {
            if (Time.time < _next) return;
            _next = Time.time + _refresh;

            Collider[] cols = Physics.OverlapSphere(transform.position, _scanRadius, enemyMask);
            HashSet<Transform> thisFrame = new();

            foreach (var c in cols)
            {
                Transform root = c.attachedRigidbody ? c.attachedRigidbody.transform : c.transform;

                thisFrame.Add(root);
                _seen[root] = Time.time;

                _sheep.ReportThreat(root, root.position, _scanRadius);
            }

            List<Transform> toRemove = null;
            foreach (var kv in _seen)
            {
                if (!thisFrame.Contains(kv.Key) && Time.time - kv.Value > 1.0f)
                {
                    (toRemove ??= new()).Add(kv.Key);
                }
            }
            if (toRemove != null)
            {
                foreach (var t in toRemove)
                {
                    _seen.Remove(t);
                    _sheep.ForgetThreat(t);
                }
            }
        }
    }

}
