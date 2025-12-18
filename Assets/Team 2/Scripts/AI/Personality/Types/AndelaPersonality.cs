using Core.AI.Sheep.Personality;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

namespace Core.AI.Sheep.Personality
{
    /// <summary>
    /// Andela is an energetic kid that likes to escape the herd, reacts playfully and acts childish
    /// </summary>
    public sealed class AndelaPersonality : BaseSheepPersonality
    {
        public override string PersonalityName => "Andela";
        
        //Tuning
        private const float WANDER_MULTIPLIER = 2.2f;
        private const float FOLLOW_DISTANCE_MULTIPLIER = 1.35f;
        private const float LOST_CHANCE_MULTIPLIER = 4.0f;
        private const float BOUNCE_ARC = 1.2f; //curve motion
        private const float SLOWER_FLEEING = 0.65f;
        
        public AndelaPersonality(SheepStateManager sheep) : base(sheep) {}

        public override Vector3 GetFollowTarget(SheepStateManager sheep, PersonalityBehaviorContext context)
        {
            float baseDistance = sheep.Archetype?.FollowDistance ?? 1.8f;
            float desired = baseDistance * FOLLOW_DISTANCE_MULTIPLIER;
            
            Vector3 dir = (context.PlayerPosition - sheep.transform.position);
            dir.y = 0f;
            dir = dir.sqrMagnitude > 0.001f ? dir.normalized : Vector3.forward;

            Vector3 side = Quaternion.Euler(0f, Random.Range(-45f, 45f), 0f) * Vector3.right;
            Vector3 bounce = side * BOUNCE_ARC;
            
            return context.PlayerPosition - dir * desired + bounce;
        }

        public override Vector3 GetGrazeTarget(SheepStateManager sheep, PersonalityBehaviorContext context)
        {
            float baseRadius = (sheep.Archetype?.IdleWanderRadius ?? 1.0f) * WANDER_MULTIPLIER;

            Vector2 rand = Random.insideUnitCircle * baseRadius;

            float angle = Random.Range(-30f, 30f);
            Vector2 curved = Quaternion.Euler(0f, angle, 0f) * new Vector3(rand.x, 0f, rand.y);
            
            Vector3 pos = sheep.transform.position;
            return new Vector3(pos.x + curved.x, pos.y, pos.z + curved.y);
        }

        public override Vector3 GetWalkAwayTarget(SheepStateManager sheep, PersonalityBehaviorContext context)
        {
            Vector2 randomArc = Random.insideUnitCircle.normalized *
                                ((sheep.Config?.WalkAwayFromHerdTicks ?? 2f) * 3f);
            
            Vector3 pos = sheep.transform.position;
            return pos + new Vector3(randomArc.x, 0f, randomArc.y);
        }

        public override void SetDestinationWithHerding(Vector3 destination, SheepStateManager sheep, PersonalityBehaviorContext context)
        {
            if (context.HasThreat)
            {
                destination = ComputeEscapeAndela(sheep, context);
            }
            
            Vector3 desired = destination -  sheep.transform.position;
            desired.y = 0f;

            float sepDist = sheep.Config?.SeparationDistance ?? 0.8f;
            float sepW = sheep.Config?.SeparationWeight ?? 1.2f;
            float alignW = sheep.Config?.AlignmentWeight ?? 0.6f;
            float clamp = sheep.Config?.SteerClamp ?? 2.5f;

            Vector3 flockSteer = FlockingUtility.Steering(
                sheep.transform,
                sheep.Neighbours,
                sepDist,
                sepW,
                alignW,
                clamp);

            Vector3 playfulArc = Quaternion.Euler(0, Random.Range(-15f, 15f), 0) * Vector3.right * 0.4f;
            
            Vector3 final = sheep.transform.position + flockSteer + desired + playfulArc;

            if (sheep.CanControlAgent())
            {
                float baseSpeed = sheep.Config?.BaseSpeed ?? 2.2f;
                bool fleeing = context.HasThreat;

                sheep.Agent.speed = fleeing ? baseSpeed * SLOWER_FLEEING : baseSpeed;
                sheep.Agent.SetDestination(final);
            }
        }

        private Vector3 ComputeEscapeAndela(SheepStateManager sheep, PersonalityBehaviorContext context)
        {
            Vector3 pos = sheep.transform.position;
            Vector3 flee = Vector3.zero;

            foreach (var t in context.Threats)
            {
                if (!t) continue;

                Vector3 away = pos - t.position;
                away.y = 0f;

                float d = Mathf.Max(away.magnitude, 0.25f);
                float w = (1f / (d * d)) * 0.4f;

                flee += away.normalized * w;
            }

            if (flee.sqrMagnitude < 0.0001f)
                return pos;
            
            flee.Normalize();

            float fleeSeconds = Mathf.Max(sheep.Config?.WalkAwayFromHerdTicks ?? 2f, 1.0f);
            float fleeDist = fleeSeconds * (sheep.Config?.BaseSpeed ?? 2.2f) * SLOWER_FLEEING;
            
            return pos + flee *  fleeDist;
        }

        public override Type GetNextState(Type currentState, Type proposedState, SheepStateManager sheep, PersonalityBehaviorContext context)
        {
            return proposedState;
        }
        
        // ------------------------------------------------------
        //  PLAYER INTERACTION
        // ------------------------------------------------------
        public override void OnPlayerAction(string actionType, SheepStateManager sheep, PersonalityBehaviorContext context)
        {
            if (actionType == "whistle" || actionType == "call")
            {
                if (Random.value < 0.5f)
                {
                    Vector3 pos = sheep.transform.position;
                    pos += Quaternion.Euler(0, Random.Range(-90f, 90f), 0) * Vector3.forward * 1.5f;
                    sheep.Agent.SetDestination(pos);
                }
                
                sheep.SetState<SheepFollowState>();
            }
        }

        public override void OnSeparatedFromHerd(SheepStateManager sheep, PersonalityBehaviorContext context)
        {
            sheep.PlayLeaveHerdVfx();
            var clip = sheep.Archetype?.LeaveHerdSound;
            if (clip)
            {
                sheep.SoundDriver.PlayMiscSound(clip, 1.0f, Random.Range(0.95f, 1.05f));
            }
        }

        public override void OnRejoinedHerd(SheepStateManager sheep, PersonalityBehaviorContext context)
        {
            sheep.PlayJoinHerdVfx();
            var clip = sheep.Archetype?.JoinHerdSound;
            if (clip)
            {
                sheep.SoundDriver.PlayMiscSound(clip, 1.0f, Random.Range(0.95f, 1.0f));
            }
        }

        public override bool CanBePetted(string currentSceneName)
        {
            return true;
        }

        public override void OnDeath(SheepStateManager sheep, PersonalityBehaviorContext context)
        {
            sheep.SetState<SheepDieState>();
        }
    }
}
