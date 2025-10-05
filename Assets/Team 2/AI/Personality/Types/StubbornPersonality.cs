using System;

using Core.Shared.StateMachine;

using UnityEngine;

using Random = UnityEngine.Random;

namespace Core.AI.Sheep.Personality.Types
{
    /// <summary>
    /// Stubborn sheep: Resists player commands, chooses own path, hard to control
    /// </summary>
    public class StubbornPersonality : NormalPersonality
    {
        public StubbornPersonality(SheepStateManager sheep) : base(sheep)
        {
            // Stubborn sheep pick a preferred spot and want to stay there
            SetNewPreferredLocation(sheep.transform.position);
        }

        public override string PersonalityName => "Stubborn";

        private float _lastRefusalTime;
        private int _consecutiveRefusals = 0;
        private Vector3 _preferredLocation;
        private bool _hasPreferredLocation = false;

        public override void OnPlayerAction(string actionType, SheepStateManager sheep, PersonalityBehaviorContext context)
        {
            // Stubborn sheep sometimes ignore player commands
            if (actionType == "whistle" || actionType == "call")
            {
                float refusalChance = 0.4f + (_consecutiveRefusals * 0.1f);

                if (Random.value < refusalChance)
                {
                    _lastRefusalTime = Time.time;
                    _consecutiveRefusals++;

                    // Instead of following, go to preferred location
                    if (_hasPreferredLocation && sheep.Agent != null)
                    {
                        sheep.Agent.SetDestination(_preferredLocation);
                    }
                }
                else
                {
                    _consecutiveRefusals = 0; // Reset refusal streak
                    // Let default behavior handle it
                    base.OnPlayerAction(actionType, sheep, context);
                }
            }
        }

        public override Type GetNextState(Type currentState, Type proposedState, SheepStateManager sheep, PersonalityBehaviorContext context)
        {
            // Resist being forced to follow if recently refused
            if (proposedState == typeof(SheepFollowState) && Time.time - _lastRefusalTime < 5f)
            {
                return typeof(SheepGrazeState); // Stay grazing instead
            }

            return proposedState; // Accept other transitions
        }

        public override Vector3 GetGrazeTarget(SheepStateManager sheep, PersonalityBehaviorContext context)
        {
            // Stubborn sheep prefer to graze near their preferred location
            if (_hasPreferredLocation)
            {
                Vector3 offset = Random.insideUnitCircle * 2f;
                return _preferredLocation + new Vector3(offset.x, 0, offset.y);
            }

            return base.GetGrazeTarget(sheep, context);
        }

        public override Vector3 GetFollowTarget(SheepStateManager sheep, PersonalityBehaviorContext context)
        {
            // Even when following, stubborn sheep don't get too close
            if (context.DistanceToPlayer < 8f)
            {
                // Find a spot that's closer to player but not too close
                Vector3 directionToPlayer = (context.PlayerPosition - sheep.transform.position).normalized;
                Vector3 stubbornTarget = sheep.transform.position + directionToPlayer * 3f;

                // Add some lateral offset to avoid direct path
                Vector3 perpendicular = Vector3.Cross(directionToPlayer, Vector3.up);
                stubbornTarget += perpendicular * Random.Range(-2f, 2f);

                return stubbornTarget;
            }

            return base.GetFollowTarget(sheep, context);
        }

        private void SetNewPreferredLocation(Vector3 currentPosition)
        {
            // Pick a spot within reasonable distance
            Vector2 randomOffset = Random.insideUnitCircle * Random.Range(5f, 15f);
            _preferredLocation = currentPosition + new Vector3(randomOffset.x, 0, randomOffset.y);
            _hasPreferredLocation = true;
        }
    }
}