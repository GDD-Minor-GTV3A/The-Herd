using UnityEngine;
using System;
using System.Collections.Generic;
using Core.Shared.StateMachine;

namespace Core.AI.Sheep.Personality
{
    /// <summary>
    /// Comprehensive interface for sheep personalities with pure delegation
    /// All sheep MUST have a personality - no null personalities allowed
    /// </summary>
    public interface ISheepPersonality
    {
        string PersonalityName { get; }

        /// <summary>
        /// Get target position when following player
        /// </summary>
        Vector3 GetFollowTarget(SheepStateManager sheep, PersonalityBehaviorContext context);

        /// <summary>
        /// Get target position when grazing
        /// </summary>
        Vector3 GetGrazeTarget(SheepStateManager sheep, PersonalityBehaviorContext context);

        /// <summary>
        /// Get target position when walking away from herd
        /// </summary>
        Vector3 GetWalkAwayTarget(SheepStateManager sheep, PersonalityBehaviorContext context);

        /// <summary>
        /// Handle movement with herding - personality controls all movement logic
        /// </summary>
        void SetDestinationWithHerding(Vector3 destination, SheepStateManager sheep, PersonalityBehaviorContext context);

        /// <summary>
        /// Allow personality to override state transitions
        /// </summary>
        Type GetNextState(Type currentState, Type proposedState, SheepStateManager sheep, PersonalityBehaviorContext context);
        

        /// <summary>
        /// Called when sheep detects a threat/enemy
        /// </summary>
        void OnThreatDetected(Vector3 threatPosition, SheepStateManager sheep, PersonalityBehaviorContext context);

        /// <summary>
        /// Called when player performs an action (whistle, etc.)
        /// </summary>
        void OnPlayerAction(string actionType, SheepStateManager sheep, PersonalityBehaviorContext context);

        /// <summary>
        /// Called when sheep gets separated from herd
        /// </summary>
        void OnSeparatedFromHerd(SheepStateManager sheep, PersonalityBehaviorContext context);

        /// <summary>
        /// Called when sheep rejoins herd
        /// </summary>
        void OnRejoinedHerd(SheepStateManager sheep, PersonalityBehaviorContext context);
        
        void OnDeath(SheepStateManager sheep, PersonalityBehaviorContext context);
    }
}