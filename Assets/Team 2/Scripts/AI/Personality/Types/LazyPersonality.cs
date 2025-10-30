using System;

using UnityEngine;

namespace Core.AI.Sheep.Personality.Types
{
    /// <summary>
    /// Lazy sheep: Moves slowly, prefers to stay still, reluctant to follow player
    /// </summary>
    public class LazyPersonality : NormalPersonality
    {
        public LazyPersonality(SheepStateManager sheep) : base(sheep) { }

        public override string PersonalityName => "Lazy";

        private float _lastMoveTime;
        private const float LAZY_MOVE_COOLDOWN = 3f;


        public override void SetDestinationWithHerding(Vector3 destination, SheepStateManager sheep, PersonalityBehaviorContext context)
        {
            // Lazy sheep resist movement if they moved recently
            if (Time.time - _lastMoveTime < LAZY_MOVE_COOLDOWN)
            {
                // Just stop the agent instead of moving
                if (sheep.Agent != null)
                {
                    sheep.Agent.isStopped = true;
                }
                return; // Don't move
            }

            _lastMoveTime = Time.time;
            // Use normal movement logic
            base.SetDestinationWithHerding(destination, sheep, context);
        }

        public override void OnPlayerAction(string actionType, SheepStateManager sheep, PersonalityBehaviorContext context)
        {
            // Lazy sheep are slow to respond to player actions
            if (actionType == "whistle" || actionType == "call")
            {
                // Add a delay before responding
                sheep.StartCoroutine(DelayedResponse(sheep, 2f));
            }
        }

        private System.Collections.IEnumerator DelayedResponse(SheepStateManager sheep, float delay)
        {
            yield return new WaitForSeconds(delay);
            // Finally respond by following
            sheep.SetState<SheepFollowState>();
        }
    }
}