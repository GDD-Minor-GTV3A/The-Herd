using UnityEngine;
using Core.AI.Sheep.Personality;
using Random = UnityEngine.Random;

namespace Core.AI.Sheep.Personality
{
    public sealed class TihomirPersonality : BaseSheepPersonality
    {
        public override string PersonalityName => "Tihomir";

        private const float FOLLOW_DISTANCE_MULTIPLIER = 0.9f;
        private const float WANDER_RADIUS_MULTIPLIER = 0.5f;

        public TihomirPersonality(SheepStateManager sheep) : base(sheep) { }

        public override Vector3 GetFollowTarget(SheepStateManager sheep, PersonalityBehaviorContext context)
        {
            float baseDistance = sheep.Archetype?.FollowDistance ?? 1.8f;
            float desired = baseDistance * FOLLOW_DISTANCE_MULTIPLIER;
            
            Vector3 dir = context.PlayerPosition - sheep.transform.position;
            dir.y = 0f;

            if (dir.sqrMagnitude < 0.0001f)
            {
                Vector2 r = Random.insideUnitCircle.normalized;
                dir = new Vector3(r.x, 0f,  r.y);
            }
            
            dir.Normalize();

            Vector3 target = context.PlayerPosition - dir * desired;
            target += Quaternion.Euler(0f, Random.Range(-15f, 15f), 0f) * (Vector3.right * (desired * 0.25f));
            
            return target;
        }

        public override Vector3 GetGrazeTarget(SheepStateManager sheep, PersonalityBehaviorContext context)
        {
            float baseRadius = (sheep.Archetype?.IdleWanderRadius ?? 1.0f) * WANDER_RADIUS_MULTIPLIER;
            Vector2 rand = Random.insideUnitCircle * baseRadius;
            return context.PlayerPosition + new Vector3(rand.x, 0f, rand.y);
        }
    }
}
