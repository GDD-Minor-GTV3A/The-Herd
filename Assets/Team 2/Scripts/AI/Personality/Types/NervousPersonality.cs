using System;
using System.Collections.Generic;

using Core.Shared.StateMachine;

using UnityEngine;

using Random = UnityEngine.Random;

namespace Core.AI.Sheep.Personality.Types
{
    /// <summary>
    /// Nervous sheep: Easily startled, flees from threats, jittery movement
    /// </summary>
    public class NervousPersonality : NormalPersonality
    {
        public NervousPersonality(SheepStateManager sheep) : base(sheep) { }

        public override string PersonalityName => "Nervous";

        private float _stressLevel = 0f;
        private float _lastPanicTime;
        private Vector3 _lastThreatPosition;


        public override void OnThreatDetected(Vector3 threatPosition, SheepStateManager sheep, PersonalityBehaviorContext context)
        {
            _lastThreatPosition = threatPosition;
            _stressLevel = Mathf.Min(1f, _stressLevel + 0.5f);
            _lastPanicTime = Time.time;

            // Nervous sheep run away from threats
            sheep.SetState<SheepWalkAwayFromHerdState>();
        }


        public override void SetDestinationWithHerding(Vector3 destination, SheepStateManager sheep, PersonalityBehaviorContext context)
        {
            // Add jittery movement when nervous
            if (_stressLevel > 0.3f)
            {
                Vector2 jitter = Random.insideUnitCircle * _stressLevel * 0.5f;
                Vector3 jitteryDestination = destination + new Vector3(jitter.x, 0, jitter.y);
                base.SetDestinationWithHerding(jitteryDestination, sheep, context);
            }
            else
            {
                base.SetDestinationWithHerding(destination, sheep, context);
            }
        }

        public float GetStressLevel() => _stressLevel;
        public Vector3 GetLastThreatPosition() => _lastThreatPosition;
    }

    /// <summary>
    /// Panic state for nervous sheep
    /// </summary>
    public class NervousPanicState : IState
    {
        private readonly SheepStateManager _sheep;
        private readonly NervousPersonality _personality;
        private float _panicEndTime;
        private Vector3 _fleeTarget;
        private const float PANIC_DURATION = 4f;

        public NervousPanicState(SheepStateManager sheep, NervousPersonality personality)
        {
            _sheep = sheep;
            _personality = personality;
        }

        public void OnStart()
        {
            _panicEndTime = Time.time + PANIC_DURATION;

            // Run away from threat or just run randomly if no specific threat
            Vector3 fleeDirection;
            Vector3 threatPos = _personality.GetLastThreatPosition();

            if (threatPos != Vector3.zero)
            {
                fleeDirection = (_sheep.transform.position - threatPos).normalized;
            }
            else
            {
                fleeDirection = Random.insideUnitCircle.normalized;
                fleeDirection = new Vector3(fleeDirection.x, 0, fleeDirection.y);
            }

            _fleeTarget = _sheep.transform.position + fleeDirection * Random.Range(5f, 10f);

            // Increase speed during panic
            if (_sheep.Agent != null)
            {
                _sheep.Agent.speed *= 1.8f;
            }
        }

        public void OnUpdate()
        {
            if (_sheep?.Agent == null) return;

            // Keep running until panic is over
            if (Time.time >= _panicEndTime)
            {
                // Try to get back to safety (near player)
                _sheep.SetState<SheepFollowState>();
                return;
            }

            // Continue fleeing
            _sheep.Agent.SetDestination(_fleeTarget);

            // If reached flee target, pick a new one
            if (!_sheep.Agent.pathPending && _sheep.Agent.remainingDistance <= 1f)
            {
                Vector2 randomDirection = Random.insideUnitCircle.normalized;
                Vector3 newDirection = new Vector3(randomDirection.x, 0, randomDirection.y);
                _fleeTarget = _sheep.transform.position + newDirection * Random.Range(3f, 6f);
            }
        }

        public void OnStop()
        {
            // Restore normal speed
            if (_sheep?.Agent != null && _sheep.Config != null)
            {
                _sheep.Agent.speed = _sheep.Config.BaseSpeed;
            }
        }
    }
}