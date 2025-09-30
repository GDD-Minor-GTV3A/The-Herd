using System.Collections.Generic;

using UnityEngine;

namespace Gameplay.Scarecrow
{
    public class DetectSheep : ScarecrowFOV
    {
        public List<Transform> VisibleTargets { get; } = new();

        protected override void FieldOfViewCheck()
        {
            VisibleTargets.Clear();

            Collider[] _rangeChecks = Physics.OverlapSphere(transform.position, _radius, _targetLayer);

            foreach (var targetCollider in _rangeChecks)
            {
                if (!targetCollider.CompareTag("Sheep"))
                    continue;

                Transform _target = targetCollider.transform;
                Vector3 _directionToTarget = (_target.position - transform.position).normalized;

                if (Vector3.Angle(transform.forward, _directionToTarget) < _angle / 2)
                {
                    float _distanceToTarget = Vector3.Distance(transform.position, _target.position);

                    if (!Physics.Raycast(transform.position, _directionToTarget, _distanceToTarget, _obstructionLayer))
                    {
                        VisibleTargets.Add(_target);
                    }
                }
            }
        }
    }
}
