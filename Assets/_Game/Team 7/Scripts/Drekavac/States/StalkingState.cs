using UnityEngine;
using UnityEngine.AI;

namespace _Game.Team_7.Scripts.Drekavac.States
{
    /// <summary>
    ///     Handles the behavior of an enemy while it's stalking a target.
    /// </summary>
    public class StalkingState : GenericEnemyState
    {
        private Vector3 _circleCenter;                      // Fixed center of circling
        private float _angleOffset;                         // Angle used for circling calculations
        private int _circleDirection;                       // 1 or -1 for clockwise/counterclockwise
        private float _nextSwitchTime;                      // Next time to switch circling direction
        private bool _isSettled;                            // True when enemy reached circle radius
        private float _stalkEndTime;                        // When to stop stalking and start hunting
        private GameObject[] _sheep = { };
        
        public StalkingState(DrekavacStateManager manager, EnemyMovementController movement, DrekavacStats stats, DrekavacAnimatorController animator, DrekavacAudioController audio) : base(manager, movement, stats, animator, audio) { }

        public override void OnStart()
        {
            _movement.SetMovementSpeed(_stats.moveSpeed);
        
            // --- Compute initial circle center from sheep ---
            _sheep = _manager.GetSheep();
            if (_sheep.Length > 0)
            {
                Vector3 avgPos = Vector3.zero;
                foreach (GameObject sheep in _sheep)
                    avgPos += sheep.transform.position;
                avgPos /= _sheep.Length;
                _circleCenter = avgPos;
            }
            else
            {
                // Fallback to player if no sheep exist
                _circleCenter = _manager.GetPlayerLocation();
            }
        
            // Position enemy at nearest point on the circle radius around the center
            Vector3 toEnemy = (_manager.transform.position - _circleCenter).normalized;
            Vector3 nearestPoint = _circleCenter + toEnemy * _manager.GetStats().circleRadius;

            if (NavMesh.SamplePosition(nearestPoint, out NavMeshHit hit, 2f, NavMesh.AllAreas)) //TODO replace hardcoded value
                _movement.MoveTo(hit.position);

            // Compute initial angleOffset based on enemy relative to circle center
            _angleOffset = Mathf.Atan2(toEnemy.z, toEnemy.x) * Mathf.Rad2Deg;
            _circleDirection = Random.value > 0.5f ? 1 : -1;  // Random initial circling direction

            ScheduleNextDirectionSwitch();
            _stalkEndTime = Time.time + Random.Range(_stats.stalkDurationRange.x, --_stats.stalkDurationRange.y);
        }

        public override void OnUpdate()
        {
            // Compute the average position of all sheep to use as circle center
            if (_sheep.Length > 0)
            {
                Vector3 avgPos = Vector3.zero;
                foreach (GameObject sheep in _sheep)
                    avgPos += sheep.transform.position;
                avgPos /= _sheep.Length;
                _circleCenter = avgPos; // Update circle center each frame
            }

            if (!_isSettled)
            {
                // Always recalc nearest point dynamically in case sheep have moved
                Vector3 toEnemy = (_manager.transform.position - _circleCenter).normalized;
                Vector3 nearestPoint = _circleCenter + toEnemy * _manager.GetStats().circleRadius;

                if (NavMesh.SamplePosition(nearestPoint, out NavMeshHit hit, 2f, NavMesh.AllAreas)) //TODO replace hardcoded value
                    _movement.MoveTo(hit.position);

                float distToCenter = Vector3.Distance(_manager.transform.position, _circleCenter);
                if (Mathf.Abs(distToCenter - _manager.GetStats().circleRadius) < 0.5f)
                {
                    _isSettled = true; // Enemy reached the circle radius
                    _audio.PlaySnarl();
                }
                else
                {
                    // Face the herd center while moving to radius
                    _movement.LookAt(_circleCenter);
                    return;
                }
            }

            // Switch circling direction at intervals
            if (Time.time >= _nextSwitchTime)
            {
                _circleDirection *= -1;
                ScheduleNextDirectionSwitch();
            }

            // Update circling angle
            _angleOffset += _stats.circleSpeed * _circleDirection * Time.deltaTime;

            // Calculate target position on circle
            float radians = _angleOffset * Mathf.Deg2Rad;
            Vector3 offset = new Vector3(Mathf.Cos(radians), 0f, Mathf.Sin(radians)) * _manager.GetStats().circleRadius;
            Vector3 targetPos = _circleCenter + offset;

            if (NavMesh.SamplePosition(targetPos, out NavMeshHit hit2, 2f, NavMesh.AllAreas)) //TODO replace hardcoded value
                _movement.MoveTo(hit2.position);

            // Face the herd center while circling
            _movement.LookAt(_circleCenter);
            _animator.SetStalking(true);

            if (Time.time >= _stalkEndTime)
                _manager.SetState<HuntingState>();
        }

        public override void OnStop()
        {
            _animator.SetStalking(false);
            _movement.Agent.updateRotation = false;
        }
    
        private void ScheduleNextDirectionSwitch()
        {
            float randomFactor = Random.Range(0.5f, 1.5f); //TODO replace hardcoded values
            _nextSwitchTime = Time.time + _stats.directionSwitchInterval * randomFactor;
        }
    }
}
