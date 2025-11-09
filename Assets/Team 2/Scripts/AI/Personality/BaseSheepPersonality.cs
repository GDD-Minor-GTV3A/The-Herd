using UnityEngine;
using System;
using System.Collections.Generic;
using Core.Shared.StateMachine;

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
            float want = Mathf.Min(0.5f, baseDistance);

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
            float baseRadius = sheep.Archetype?.IdleWanderRadius ?? 1.0f;
            Vector2 rand = Random.insideUnitCircle * baseRadius;
            return sheep.transform.position + new Vector3(rand.x, 0f, rand.y);
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
                ? ComputeEscapeDestination(sheep, context) : destination;
            Vector3 desired = goal - sheep.transform.position;
            desired.y = 0f;

            float sepDist = sheep.Config?.SeparationDistance ?? 0.8f;
            float sepW = sheep.Config?.SeparationWeight ?? 1.2f;
            float alignW = sheep.Config?.AlignmentWeight ?? 0.6f;
            float clamp = sheep.Config?.SteerClamp ?? 2.5f;

            Vector3 steer = FlockingUtility.Steering(
                sheep.transform,
                sheep.Neighbours,
                sepDist,
                sepW,
                alignW,
                clamp
            );

            Vector3 final = sheep.transform.position + desired + steer;

            if (sheep.CanControlAgent())
            {
                float baseSpeed = sheep.Config?.BaseSpeed ?? 2.2f;
                bool isFLeeing = context.HasThreat;
                
                sheep.Agent.speed = isFLeeing ? baseSpeed * 1.5f : baseSpeed;
                sheep.Agent.SetDestination(final);

                if (isFLeeing)
                {
                    Vector3 look = final - sheep.transform.position;
                    look.y = 0f;
                    if (look.sqrMagnitude > 0.0001f)
                    {
                        var q = Quaternion.LookRotation(look);
                        sheep.transform.rotation = Quaternion.Slerp(sheep.transform.rotation, q, Time.deltaTime * 10f);
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

        #endregion


        #region Event Hooks - Default implementations

        public virtual void OnThreatDetected(Vector3 threatPosition, SheepStateManager sheep, PersonalityBehaviorContext context)
        {
            // Default behavior: run away
            sheep.SetState<SheepWalkAwayFromHerdState>();
        }

        public virtual void OnPlayerAction(string actionType, SheepStateManager sheep, PersonalityBehaviorContext context)
        {
            // Default behavior for player actions
            if (actionType == "whistle" || actionType == "call")
            {
                sheep.SetState<SheepFollowState>();
            }
        }

        public virtual void OnSeparatedFromHerd(SheepStateManager sheep, PersonalityBehaviorContext context)
        {
            // Not handled by default
        }

        public virtual void OnRejoinedHerd(SheepStateManager sheep, PersonalityBehaviorContext context)
        {
            // Not handled by default
        }

        public virtual void OnDeath(SheepStateManager sheep, PersonalityBehaviorContext context)
        {
            sheep.SetState<SheepDieState>();
        }

        #endregion


    }
}