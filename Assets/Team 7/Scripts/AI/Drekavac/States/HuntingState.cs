using UnityEngine;

namespace Team_7.Scripts.AI.Drekavac.States
{
    /// <summary>
    ///     Handles the behavior of an enemy while it's trying to home in on a target.
    /// </summary>
    public class HuntingState : DrekavacState
    {
        public HuntingState(DrekavacStateManager manager, EnemyMovementController movement, DrekavacStats stats, DrekavacAnimatorController animator, AudioController audio) : base(manager, movement, stats, animator, audio) { }

        public override void OnStart()
        {
            _movement.ToggleAgent(true);
            _movement.SetMovementSpeed(_stats.sprintSpeed);
        }

        public override void OnUpdate()
        {
            var sheepObjects = _manager.GetSheep();
            if (sheepObjects.Length == 0) 
                return;

            GameObject? closestSheep = null;
            float closestDist = Mathf.Infinity;

            foreach (GameObject sheep in sheepObjects)
            {
                float dist = Vector3.Distance(_manager.transform.position, sheep.transform.position);
                if (!(dist < closestDist))
                    continue;

                closestDist = dist;
                closestSheep = sheep;
            }

            if (closestSheep is null)
                return;

            var closestPosition = closestSheep.transform.position;
            _movement.MoveTo(closestPosition);
            _movement.LookAt(closestPosition);
        }
    }
}
