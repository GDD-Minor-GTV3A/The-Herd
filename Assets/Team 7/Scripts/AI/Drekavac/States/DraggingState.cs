using Core.AI.Sheep;
using Core.AI.Sheep.Event;
using Core.Events;

using UnityEngine;
using UnityEngine.AI;

namespace Team_7.Scripts.AI.Drekavac.States
{
    /// <summary>
    ///     Handles the behavior of an enemy while it's dragging an object.
    /// </summary>
    public class DraggingState : DrekavacState
    {
        public DraggingState(DrekavacStateManager manager, EnemyMovementController movement, DrekavacStats stats, DrekavacAnimatorController animator, AudioController audio)
            : base(manager, movement, stats, animator, audio) { }

        public override void OnStart()
        {
            _animator.SetDragging(true);
            _movement.SetMovementSpeed(_manager.GetStats().dragSpeed);
            _audio.PlayClip(_manager.GetStats().chompSound);
        }

        public override void OnUpdate()
        {
            if (_manager.GetGrabbedObject() is null)
            {
                _manager.SetState<HuntingState>();
                return;
            }

            if (Vector3.Distance(_manager.transform.position, _manager.GetPlayerLocation()) >
                _manager.GetStats().despawnDistance)
            {
                _manager.GetGrabbedObject().TryGetComponent<SheepStateManager>(out var sheepManager);
                EventManager.Broadcast(new SheepDamageEvent(sheepManager, 1000, sheepManager.transform.position, source: _manager.gameObject));
                
                _manager.ReleaseGrabbedObject();
                _manager.DestroySelf();
                return;
            }


            // Compute average position of remaining sheep (excluding grabbed sheep)
            Vector3 sheepCenter = Vector3.zero;
            int count = 0;
            foreach (GameObject sheep in _manager.GetSheep())
            {
                if (sheep == _manager.GetGrabbedObject())
                    continue;

                sheepCenter += sheep.transform.position;
                count++;
            }
            sheepCenter = count > 0 ? sheepCenter / count : _manager.GetPlayerLocation();

            var position = _manager.transform.position;
        
            // Run in opposite direction of sheep herd
            Vector3 awayDir = (position - sheepCenter).normalized;
            Vector3 escapeTarget = position + awayDir * _stats.dragAwayDistance;

            if (NavMesh.SamplePosition(escapeTarget, out NavMeshHit hit, 5f, NavMesh.AllAreas)) //TODO replace hardcoded value with variable
                _movement.MoveTo(hit.position);

            // Face backwards toward the herd (look at herd, move away)
            _movement.LookAt(sheepCenter);
        }

        public override void OnStop()
        {
            _animator.SetDragging(false);
        }
    }
}
