using UnityEngine;
using UnityEngine.AI;

namespace Team_7.Scripts.AI.Phantom.States
{
    public class WanderingState : PhantomState
    {
        private bool _running;

        public WanderingState(PhantomStateManager manager, EnemyMovementController movement, PhantomStats stats, PhantomAnimatorController animator, AudioController audio)
            : base(manager, movement, stats, animator, audio) { }

        public override void OnStart()
        {
            _movement.Agent.speed = _stats.moveSpeed;
        }

        public override void OnUpdate()
        {
            if (_running && _movement.Agent.remainingDistance < 3 && !IsInVisionCone(_manager.GetPlayerTransform(), _manager.transform.position, _stats.DamageAngle))
            {
                _movement.SetMovementSpeed(_stats.moveSpeed);
                _running = false;
                _manager.ResetShotCounter();
            }

            var distance = Vector3.Distance(_manager.GetPlayerTransform().position, _manager.transform.position);
            if (distance < _stats.shootRange && !_manager.IsBeingLookedAt() && !_running && _manager.GetShotCounter() < _stats.shotsBeforeDashing)
            {
                _manager.SetState<ShootingState>();
                return;
            }
            
            if (distance > _stats.shootRange && !_running)
                _movement.MoveTo(_manager.GetPlayerTransform().position);

            if (_running)
                return;

            // Run to an area outside the players vision.
            Vector3 safePos;
            if (FindSafeSpot(out safePos) && _manager.GetShotCounter() != 0)
            {
                _manager.ToggleVisibility(false);
                _movement.SetMovementSpeed(_stats.sprintSpeed);
                _movement.MoveTo(safePos);
                _running = true;
            }
        }

        /// <summary>
        ///     Finds an area that is at least some distance from the player and outside the players view cone.
        /// </summary>
        /// <param name="result">The position to move towards</param>
        /// <returns>True if it has found a safe spot to path to</returns>
        bool FindSafeSpot(out Vector3 result)
        {
            for (int i = 0; i < 20; i++)
            {
                Vector3 randomDir = Random.insideUnitSphere * _stats.repositionDistance;
                randomDir += _manager.GetPlayerTransform().position;

                NavMeshHit hit;
                if (NavMesh.SamplePosition(randomDir, out hit, 2f, NavMesh.AllAreas))
                {
                    // Check if new position is outside the player's vision cone and not too close.
                    if (!IsInVisionCone(_manager.GetPlayerTransform(), hit.position, _stats.DamageAngle) && 
                        Vector3.Distance(_manager.GetPlayerTransform().position, hit.position) > _stats.minRepositionPlayerDistance)
                    {
                        result = hit.position;
                        return true;
                    }
                }
            }
            result = _manager.transform.position;
            return false;
        }
        
        bool IsInVisionCone(Transform player, Vector3 targetPosition, float viewAngle)
        {
            Vector3 dirToTarget = (targetPosition - player.position).normalized;
            float angle = Vector3.Angle(player.forward, dirToTarget);
            return angle < viewAngle * 0.5f;
        }
        
        public override void OnStop()
        {
            _manager.ToggleVisibility(true);
        }
    }
}
