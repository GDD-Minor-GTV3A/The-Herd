using UnityEngine;

namespace Core.AI.Sheep.Personality
{
    public sealed class YaroPersonality : BaseSheepPersonality
    {
        public override string PersonalityName => "Yaro";

        private const float FOLLOW_DISTANCE_MULTIPLIER = 1.4f;
        private const float EDGE_WILL = 0.8f;
        private const float SEPARATION_DISTANCE_MULTIPLIER = 1.4f;
        private const float SEPARATION_WEIGHT_MULTIPLIER = 1.5f;
        private const float ALIGNMENT_WEIGHT_MULTIPLIER = 0.5f;

        public YaroPersonality(SheepStateManager sheep) : base(sheep) { }

        public override Vector3 GetFollowTarget(SheepStateManager sheep, PersonalityBehaviorContext context)
        {
            Vector3 baseTarget = base.GetFollowTarget(sheep, context);

            Vector3 radial = baseTarget - context.PlayerPosition;
            radial.y = 0f;

            if (radial.sqrMagnitude < 0.0001f)
            {
                radial = sheep.transform.position - context.PlayerPosition;
                radial.y = 0f;

                if (radial.sqrMagnitude < 0.0001f)
                {
                    Vector2 r = Random.insideUnitCircle;
                    radial = new Vector3(r.x, 0f, r.y);
                }
            }
            
            float currentRadius = radial.magnitude;
            radial /= Mathf.Max(currentRadius, 0.0001f);

            float extraDistance = currentRadius * (FOLLOW_DISTANCE_MULTIPLIER - 1f);
            Vector3 pushed = baseTarget + radial * extraDistance;
            
            return pushed;
        }

        public override Vector3 GetGrazeTarget(SheepStateManager sheep, PersonalityBehaviorContext context)
        {
            Vector3 baseTarget = base.GetGrazeTarget(sheep, context);

            Vector3 radial = baseTarget - context.PlayerPosition;
            radial.y = 0f;

            if (radial.sqrMagnitude < 0.0001f)
            {
                radial = sheep.transform.position - context.PlayerPosition;
                radial.y = 0f;
            }

            if (radial.sqrMagnitude < 0.0001f)
            {
                Vector2 r = Random.insideUnitCircle;
                radial = new Vector3(r.x, 0f, r.y);
            }
            
            radial.Normalize();
            Vector3 pushed = baseTarget + radial * EDGE_WILL;
            
            return pushed;
        }

        public override void SetDestinationWithHerding(Vector3 destination, SheepStateManager sheep, PersonalityBehaviorContext context)
        {
            Vector3 goal = context.HasThreat && context.Threats.Count > 0
                ? ComputeEscapeDestination(sheep, context)
                : destination;

            Vector3 playerPos = context.PlayerPosition;
            playerPos.y = 0f;

            float avoidRadius = sheep.Config?.PlayerAvoidRadius ?? 1.5f;
            float avoidRadiusSqr = avoidRadius * avoidRadius;

            Vector3 goalFlat = goal;
            goalFlat.y = 0f;
            
            Vector3 fromPlayerToGoal = goalFlat - playerPos;
            float sqrDistanceToPlayer = fromPlayerToGoal.sqrMagnitude;

            if (sqrDistanceToPlayer < avoidRadiusSqr)
            {
                if (sqrDistanceToPlayer < 0.0001f)
                {
                    fromPlayerToGoal = sheep.transform.position - playerPos;
                    fromPlayerToGoal.y = 0f;

                    if (fromPlayerToGoal.sqrMagnitude < 0.0001f)
                    {
                        Vector2 r = Random.insideUnitCircle.normalized;
                        fromPlayerToGoal = new Vector3(r.x, 0f, r.y);
                    }
                }
                
                fromPlayerToGoal.Normalize();
                goal = playerPos + fromPlayerToGoal * avoidRadius;
            }
            
            Vector3 desired = goal - sheep.transform.position;
            desired.y = 0f;

            float baseSepDist = sheep.Config?.SeparationDistance ?? 0.8f;
            float baseSepW = sheep.Config?.SeparationWeight ?? 1.2f;
            float baseAlignW = sheep.Config?.AlignmentWeight ?? 0.6f;
            float clamp = sheep.Config?.SteerClamp ?? 2.5f;

            float sepDist = baseSepDist * SEPARATION_DISTANCE_MULTIPLIER;
            float sepW = baseSepW * SEPARATION_WEIGHT_MULTIPLIER;
            float alignW = baseAlignW * ALIGNMENT_WEIGHT_MULTIPLIER;

            Vector3 flockSteer = FlockingUtility.Steering(
                sheep.transform,
                sheep.Neighbours,
                sepDist,
                sepW,
                alignW,
                clamp);
            
            Vector3 repulsion = Vector3.zero;
            Vector3 fromPlayerToSheep = sheep.transform.position - playerPos;
            fromPlayerToSheep.y = 0f;

            float distToPlayer = fromPlayerToSheep.magnitude;
            if (distToPlayer < avoidRadius && distToPlayer > 0.001f)
            {
                float t = 1f - (distToPlayer / avoidRadius);
                float repulsionWeight = sheep.Config?.PlayerAvoidWeight ?? 1.5f;
                Vector3 dir = fromPlayerToSheep / distToPlayer;
                repulsion = dir * (t * repulsionWeight);
            }
            
            Vector3 final = sheep.transform.position + repulsion + desired + flockSteer;

            if (sheep.CanControlAgent())
            {
                float baseSpeed = sheep.Config?.BaseSpeed ?? 2.2f;
                bool isFleeing = context.HasThreat;

                sheep.Agent.speed = isFleeing ? baseSpeed * 1.5f : baseSpeed;
                sheep.Agent.SetDestination(final);
                if (isFleeing)
                {
                    Vector3 look = final - sheep.transform.position;
                    look.y = 0f;

                    if (look.sqrMagnitude > 0.0001f)
                    {
                        Quaternion q = Quaternion.LookRotation(look);
                        sheep.transform.rotation = Quaternion.Slerp(
                            sheep.transform.rotation,
                            q,
                            Time.deltaTime * 10f);
                    }
                }
            }
        }
    }
}
