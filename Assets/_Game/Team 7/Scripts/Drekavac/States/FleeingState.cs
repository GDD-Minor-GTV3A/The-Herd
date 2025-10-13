using UnityEngine;
using UnityEngine.AI;

namespace _Game.Team_7.Scripts.Drekavac.States
{
    /// <summary>
    ///     Handles the behavior of an enemy while it's trying to run away.
    /// </summary>
    public class FleeingState : GenericEnemyState
    {
        private Vector3 fleeTarget;                       // Escape target position

        public FleeingState(DrekavacStateManager manager, EnemyMovementController movement, DrekavacStats stats, DrekavacAnimatorController animator, AudioController audio) : base(manager, movement, stats, animator, audio) { }

        public override void OnStart()
        {
            _movement.SetMovementSpeed(_stats.fleeSpeed);
            
            // Drop object if dragging
            if (_manager.GetGrabbedObject() is not null)
                _manager.ReleaseGrabbedObject();

            var position = _manager.gameObject.transform.position;
            Vector3 awayDir = (position - _manager.GetPlayerLocation()).normalized;
            Vector3 rawTarget = position + awayDir * Mathf.Max(_stats.fleeDistance, _stats.circleRadius * 2f);

            // Find a valid NavMesh point in the intended direction
            if (NavMesh.SamplePosition(rawTarget, out NavMeshHit hit, 10f, NavMesh.AllAreas)) //TODO replace hardcoded values
            {
                Vector3 destination = hit.position;
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
                _manager.DestroySelf();
            }
        }

        public override void OnUpdate()
        {
            if (_movement.Agent.hasPath)
                _movement.LookAt(_movement.Agent.steeringTarget); // face where running

            // Check if agent really moved and reached destination
            if (!_movement.Agent.pathPending && 
                _movement.Agent.hasPath &&
                _movement.Agent.remainingDistance <= _movement.Agent.stoppingDistance + 0.5f) //TODO replace hardcoded value with variable
                _manager.Despawn();
        }
    }
}
