using Core.AI.Sheep.Personality;
using Core.Shared.StateMachine;
using UnityEngine;

namespace Core.AI.Sheep.Personality
{
    public class SonjaPersonality : BaseSheepPersonality
    {
        public override string PersonalityName => "Sonja";

        private const float SONJA_SPEED_MULTIPLIER = 0.45f;

        private const float SONJA_SEPARATION_MULTIPLIER = 0.2f;

        public SonjaPersonality(SheepStateManager sheep) : base(sheep) {}
        
        /// <summary>
        /// Sonja needs to stay close to the player, uses different offset than other sheep to avoid stacking
        /// </summary>
        public override Vector3 GetFollowTarget(SheepStateManager sheep, PersonalityBehaviorContext context)
        {
            float desired = (sheep.Archetype?.FollowDistance ?? 1.8f) * 0.8f;
            
            Vector3 dir = (context.PlayerPosition - sheep.transform.position);
            dir.y = 0f;
            dir = dir.sqrMagnitude > 0.001f ? dir.normalized : Vector3.forward;
            
            return context.PlayerPosition - dir * desired;
        }

        public override void SetDestinationWithHerding(Vector3 destination, SheepStateManager sheep, PersonalityBehaviorContext context)
        {
            Vector3 goal = context.HasThreat && context.Threats.Count > 0
                ? ComputeEscapeDestination(sheep, context)
                : destination;
            
            Vector3 playerPosition = context.PlayerPosition;
            playerPosition.y = 0f;
            float avoidRadius = sheep.Config?.PlayerAvoidRadius ?? 1.5f;
            float avoidRadiusSqr = avoidRadius * avoidRadius;

            Vector3 goalFlat = goal;
            goalFlat.y = 0f;
            Vector3 fromPlayerToGoal = goalFlat - playerPosition;

            if (fromPlayerToGoal.sqrMagnitude < avoidRadiusSqr)
            {
                fromPlayerToGoal.Normalize();
                goal = playerPosition + fromPlayerToGoal *  avoidRadius;
            }
            
            Vector3 desired = goal - sheep.transform.position;
            desired.y = 0f;

            float sepDist = sheep.Config?.SeparationDistance ?? 0.8f;
            float sepW = (sheep.Config?.SeparationWeight ?? 1.2f) * SONJA_SEPARATION_MULTIPLIER;
            float alignW = (sheep.Config?.AlignmentWeight ?? 0.6f) * 0.3f;
            float clamp = sheep.Config?.SteerClamp ?? 2.5f;

            Vector3 flockSteer = FlockingUtility.Steering(
                sheep.transform,
                sheep.Neighbours,
                sepDist,
                sepW,
                alignW,
                clamp);
            
            Vector3 repulsion = Vector3.zero;
            Vector3 fromPlayerToSheep = sheep.transform.position - playerPosition;
            fromPlayerToSheep.y = 0f;
            float distToPlayer = fromPlayerToSheep.magnitude;

            if (distToPlayer < avoidRadius && distToPlayer > 0.001f)
            {
                float t = 1f - (distToPlayer / avoidRadius);
                float repulsionW = sheep.Config?.PlayerAvoidWeight ?? 1.5f;
                repulsion = fromPlayerToSheep.normalized * (t * repulsionW);
            }
            
            Vector3 final = sheep.transform.position + desired + flockSteer + repulsion;

            if (sheep.CanControlAgent())
            {
                float baseSpeed = sheep.Config?.BaseSpeed ?? 2.2f;
                bool isFleeing = context.HasThreat;

                float speedMultiplier = isFleeing ? 1.0f : SONJA_SPEED_MULTIPLIER;
                
                sheep.Agent.speed = baseSpeed * speedMultiplier;
                sheep.Agent.SetDestination(final);

                if (isFleeing)
                {
                    Vector3 look = final - sheep.transform.position;
                    look.y = 0f;
                    if (look.sqrMagnitude > 0.0001f)
                    {
                        var q = Quaternion.LookRotation(look);
                        sheep.transform.rotation = Quaternion.Slerp(sheep.transform.rotation, q, Time.deltaTime * 5f);
                    }
                }
            }
        }
    }
}
