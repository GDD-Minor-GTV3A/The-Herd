using System;
using System.Collections.Generic;

using Core.Shared.StateMachine;

using UnityEngine;

using Random = UnityEngine.Random;

namespace Core.AI.Sheep.Personality.Types
{
    /// <summary>
    /// Energetic sheep: Moves fast, bursts away from player, hyperactive behavior
    /// </summary>
    public class EnergeticPersonality : NormalPersonality
    {
        public EnergeticPersonality(SheepStateManager sheep) : base(sheep) { }

        public override string PersonalityName => "Energetic";

        private float _lastBurstTime;
        private const float BURST_COOLDOWN = 8f;
        private const float BURST_CHANCE = 0.3f;

        public override Vector3 GetFollowTarget(SheepStateManager sheep, PersonalityBehaviorContext context)
        {
            // Sometimes burst away from player instead of following
            if (CanBurst() && Random.value < BURST_CHANCE)
            {
                _lastBurstTime = Time.time;
                // Get a target that's away from the player
                Vector3 awayDirection = (sheep.transform.position - context.PlayerPosition).normalized;
                return sheep.transform.position + awayDirection * 5f;
            }

            return base.GetFollowTarget(sheep, context); // Use normal behavior
        }

        public override void OnPlayerAction(string actionType, SheepStateManager sheep, PersonalityBehaviorContext context)
        {
            // Energetic sheep over-react to player actions - just follow more eagerly
            if (actionType == "whistle")
            {
                sheep.SetState<SheepFollowState>();
            }
        }

        private bool CanBurst()
        {
            return Time.time - _lastBurstTime > BURST_COOLDOWN;
        }
    }
}