using Core.AI.Sheep;
using Core.AI.Sheep.Event;
using Core.Events;

using UnityEngine;
using UnityEngine.AI;

namespace Team_7.Scripts.AI.Drekavac.States
{
    /// <summary>
    ///     Handles the behavior of an enemy while it's trying to run away.
    /// </summary>
    public class FleeingState : DrekavacState
    {
        private Vector3 fleeTarget;                       // Escape target position

        public FleeingState(DrekavacStateManager manager, EnemyMovementController movement, DrekavacStats stats, DrekavacAnimatorController animator, AudioController audio)
            : base(manager, movement, stats, animator, audio) { }

        public override void OnStart()
        {
            _movement.SetMovementSpeed(_stats.fleeSpeed);
            
            // Drop object if dragging
            /*if (_manager.GetGrabbedObject() is not null)
                _manager.ReleaseGrabbedObject();*/

            var position = _manager.gameObject.transform.position;
            //Vector3 awayDir = (position - _manager.GetPlayerLocation()).normalized;
            //Vector3 rawTarget = position + awayDir * Mathf.Max(_stats.fleeDistance, _stats.circleRadius * 2f);

            // Find a valid NavMesh point in the intended direction
            if (TryFindBestFleePosition(position, _manager.GetPlayerLocation(), out var destination))
            {
                _movement.Agent.isStopped = false;
                _movement.ResetAgent();
                _movement.Agent.updateRotation = false; // let LookAt handle facing
                _movement.ToggleAgent(true);
                _movement.MoveTo(destination);
            }
            else
            {
                Debug.LogWarning("DrekavacAI: Could not find valid abort position.");
                // If no path, fallback to despawn directly
                _manager.ReleaseGrabbedObject();
                _manager.DestroySelf();
            }
        }
        
        private bool TryFindBestFleePosition(
            Vector3 enemyPos,
            Vector3 playerPos,
            out Vector3 bestPosition)
        {
            const int attempts = 50;
            bestPosition = Vector3.zero;
            float bestScore = 0;

            for (int i = 0; i < attempts; i++)
            {
                float angle = i * (360f / attempts);
                Vector3 dir = Quaternion.Euler(0, angle, 0) * Vector3.forward;
                Vector3 target = enemyPos + dir * _stats.fleeDistance;

                if (!NavMesh.SamplePosition(target, out NavMeshHit hit, _stats.circleRadius, NavMesh.AllAreas))
                    continue;
                
                NavMeshPath path = new ();
                if (!NavMesh.CalculatePath(enemyPos, hit.position, NavMesh.AllAreas, path) ||
                    path.status != NavMeshPathStatus.PathComplete)
                    continue;

                float distanceFromPlayer = Vector3.Distance(hit.position, playerPos);
                
                float moveTowardPenalty =
                    Vector3.Dot(dir, (playerPos - enemyPos).normalized) > 0 ? -5f : 0f;

                float score = distanceFromPlayer + moveTowardPenalty;

                if (score > bestScore)
                {
                    bestScore = score;
                    bestPosition = hit.position;
                }
            }

            return bestScore > 0;
        }

        public override void OnUpdate()
        {
            if (_movement.Agent.hasPath)
                _movement.LookAt(_movement.Agent.steeringTarget); // face where running

            // Check if agent really moved and reached destination
            if (!_movement.Agent.pathPending &&
                _movement.Agent.hasPath &&
                _movement.Agent.remainingDistance <=
                _movement.Agent.stoppingDistance + 3f) //TODO replace hardcoded value with variable
            {
                if (_manager.TryGetGrabbedObject(out var grabbedObject))
                {
                    grabbedObject.TryGetComponent<SheepStateManager>(out var sheepManager);
                    EventManager.Broadcast(new SheepDamageEvent(sheepManager, 1000, sheepManager.transform.position, source: _manager.gameObject));
                    _manager.ReleaseGrabbedObject();
                }

                _manager.DestroySelf();
            }
        }
    }
}
