using UnityEngine;

namespace Gameplay.Scarecrow
{
    public class FieldOfView : ScarecrowFOV
    {
        [SerializeField] private GameObject _playerRef;
        [field: SerializeField] public bool CanSeePlayer { get; private set; }

        protected override void FieldOfViewCheck()
        {
            Collider[] rangeChecks = Physics.OverlapSphere(transform.position, _radius, _targetLayer);

            if (rangeChecks.Length != 0)
            {
                Transform target = rangeChecks[0].transform;
                Vector3 directionToTarget = (target.position - transform.position).normalized;

                if (Vector3.Angle(transform.forward, directionToTarget) < _angle / 2)
                {
                    float distanceToTarget = Vector3.Distance(transform.position, target.position);

                    if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, _obstructionLayer))
                        CanSeePlayer = true;
                    else
                        CanSeePlayer = false;
                }
                else
                    CanSeePlayer = false;
            }
            else if (CanSeePlayer)
                CanSeePlayer = false;
        }
    }
}
