using UnityEngine;
using System;
using System.Collections.Generic;
using Core.Shared.StateMachine;

using Unity.Properties;

using Random = UnityEngine.Random;

namespace Core.AI.Sheep.Personality
{
    /// <summary>
    /// Base implementation of ISheepPersonality that provides default behavior
    /// Inherit from this to only override specific behaviors
    /// </summary>
    public abstract class BaseSheepPersonality : ISheepPersonality
    {
        protected readonly SheepStateManager _sheep;

        protected BaseSheepPersonality(SheepStateManager sheep)
        {
            _sheep = sheep;
        }

        public abstract string PersonalityName { get; }

        #region Core Behavior - Default implementations

        private const float DEFAULT_FOLLOW_DISTANCE = 1.8f;
        private const float DEFAULT_MAX_LOST_DISTANCE_FROM_HERD = 10f;

        public virtual Vector3 GetFollowTarget(SheepStateManager sheep, PersonalityBehaviorContext context)
        {
            float baseDistance = sheep.Archetype?.FollowDistance ?? DEFAULT_FOLLOW_DISTANCE;
            float want = baseDistance;

            Vector3 dir = (context.PlayerPosition - sheep.transform.position);
            dir.y = 0f;
            dir = dir.sqrMagnitude > 0.001f ? dir.normalized : Vector3.forward;

            Vector3 target = context.PlayerPosition - dir * want;

            // Reduce stacking
            target += Quaternion.Euler(0f, Random.Range(-35f, 35f), 0f) * (Vector3.right * (want * 0.5f));
            return target;
        }

        public virtual Vector3 GetGrazeTarget(SheepStateManager sheep, PersonalityBehaviorContext context)
        {
            float baseRadius   = sheep.Archetype?.IdleWanderRadius ?? 1.0f;
            float minSpacing   = baseRadius * 0.75f;
            float minSpacingSq = minSpacing * minSpacing;
            
            if (sheep.IsStraggler)
            {
                Vector2 rand = Random.insideUnitCircle * baseRadius;
                return sheep.transform.position + new Vector3(rand.x, 0f, rand.y);
            }
            
            Vector3 center = context.PlayerPosition;
            Vector3 half   = context.PlayerHalfExtents;

            var neighbours = sheep.Neighbours;

            const int MAX_ATTEMPTS = 8;
            for (int attempt = 0; attempt < MAX_ATTEMPTS; attempt++)
            {
                float x = Random.Range(-half.x, half.x);
                float z = Random.Range(-half.z, half.z);
                Vector3 candidate = center + new Vector3(x, 0f, z);

                bool tooClose = false;

                if (neighbours != null)
                {
                    for (int i = 0; i < neighbours.Count; i++)
                    {
                        Transform n = neighbours[i];
                        if (n == null) continue;

                        Vector3 diff = n.position - candidate;
                        diff.y = 0f;

                        if (diff.sqrMagnitude < minSpacingSq)
                        {
                            tooClose = true;
                            break;
                        }
                    }
                }

                if (!tooClose)
                    return candidate;
            }
            
            Vector2 fallback = Random.insideUnitCircle * baseRadius;
            return sheep.transform.position + new Vector3(fallback.x, 0f, fallback.y);
        }


        public virtual Vector3 GetWalkAwayTarget(SheepStateManager sheep, PersonalityBehaviorContext context)
        {
            float maxLostDistanceFromHerd = sheep.Config?.MaxLostDistanceFromHerd ?? DEFAULT_MAX_LOST_DISTANCE_FROM_HERD;
            Vector2 rand = Random.insideUnitCircle * maxLostDistanceFromHerd;

            Vector3 playerHalfExtents = context.PlayerHalfExtents;
            Vector3 playerHalf = new(
                rand.x < 0 ? playerHalfExtents.x * -1 : playerHalfExtents.x,
                0f,
                rand.y < 0 ? playerHalfExtents.z * -1 : playerHalfExtents.z
            );

            return sheep.transform.position + playerHalf + new Vector3(rand.x, 0f, rand.y);
        }

        public virtual void SetDestinationWithHerding(Vector3 destination, SheepStateManager sheep, PersonalityBehaviorContext context)
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
                        Vector2 rand = UnityEngine.Random.insideUnitCircle.normalized;
                        fromPlayerToGoal = new Vector3(rand.x, 0f, rand.y);
                    }
                }
                
                fromPlayerToGoal.Normalize();
                goal = playerPos + fromPlayerToGoal * avoidRadius;
            }

            Vector3 desired = goal - sheep.transform.position;
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
                clamp
            );

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
            
            Vector3 final = sheep.transform.position + desired + flockSteer + repulsion;

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
                        var q = Quaternion.LookRotation(look);
                        sheep.transform.rotation = Quaternion.Slerp(sheep.transform.rotation, 
                            q,
                            Time.deltaTime * 10f);
                    }
                }
            }
        }

        protected virtual Vector3 ComputeEscapeDestination(SheepStateManager sheep, PersonalityBehaviorContext ctx)
        {
            Vector3 myPos = sheep.transform.position;
            Vector3 flee = Vector3.zero;

            foreach (var t in ctx.Threats)
            {
                if (!t) continue;
                Vector3 away = myPos - t.position;
                away.y = 0f;
                float d = Mathf.Max(away.magnitude, 0.25f);

                float w = 1f / (d * d);

                if (ctx.ThreatRadius.TryGetValue(t, out float r) && r > 0.01f)
                {
                    float boost = Mathf.Clamp01(1f + (r - d) / r);
                    w *= boost;
                }

                flee += away.normalized * w;
            }

            if (flee.sqrMagnitude < 0.0001f)
            {
                return myPos;
            }

            flee.Normalize();

            float fleeSeconds = Mathf.Max(sheep.Config?.WalkAwayFromHerdTicks ?? 2f, 1.5f);
            float fleeDist = fleeSeconds * (sheep.Config?.BaseSpeed ?? 2.2f);

            return myPos + flee * fleeDist;
        }

        public virtual Type GetNextState(Type currentState, Type proposedState, SheepStateManager sheep, PersonalityBehaviorContext context)
        {
            // Default behavior: accept all state transitions
            return proposedState;
        }

        public virtual bool CanBePetted(string currentSceneName)
        {
            /*if (currentSceneName.Equals("Village", System.StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }*/
            return true;
            //return false;
        }

        #endregion


        #region Event Hooks - Default implementations

        public virtual void OnThreatDetected(Vector3 threatPosition, SheepStateManager sheep, PersonalityBehaviorContext context)
        {
            // Default behavior: run away
            sheep.SetState<SheepWalkAwayFromHerdState>();
        }

        public virtual void OnPlayerAction(string actionType, SheepStateManager sheep, PersonalityBehaviorContext context)
        {
            Debug.Log($"PERSONALITY ACTION: {actionType} on {sheep.name}");
            // Default behavior for player actions
            if (actionType == "whistle" || actionType == "call")
            {
                sheep.SetState<SheepFollowState>();
            }
        }

        public virtual void OnSeparatedFromHerd(SheepStateManager sheep, PersonalityBehaviorContext context)
        {
            sheep.PlayLeaveHerdVfx();
            var clip = sheep.Archetype?.LeaveHerdSound;
            if (clip)
            {
                sheep.SoundDriver.PlayMiscSound(
                    clip,
                    1.0f,
                    Random.Range(0.95f, 1.05f));
            }
        }

        public virtual void OnRejoinedHerd(SheepStateManager sheep, PersonalityBehaviorContext context)
        {
            sheep.PlayJoinHerdVfx();
            var clip = sheep.Archetype?.JoinHerdSound;
            if (clip)
            {
                sheep.SoundDriver.PlayMiscSound(
                    clip,
                    1.0f,
                    Random.Range(0.95f, 1.05f));
            }
        }

        public virtual void OnDeath(SheepStateManager sheep, PersonalityBehaviorContext context)
        {
            sheep.SetState<SheepDieState>();
        }

        #endregion


    }
}