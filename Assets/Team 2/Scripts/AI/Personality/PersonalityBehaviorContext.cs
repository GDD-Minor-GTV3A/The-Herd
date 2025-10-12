using System.Collections.Generic;
using UnityEngine;

namespace Core.AI.Sheep.Personality
{
    /// <summary>
    /// Context data passed to personality behaviors for decision making
    /// </summary>
    public class PersonalityBehaviorContext
    {
        public Vector3 PlayerPosition { get; set; }
        public Vector3 PlayerHalfExtents { get; set; }
        public float DistanceToPlayer { get; set; }
        public bool IsPlayerMoving { get; set; }
        public Vector3 ThreatPosition { get; set; }
        public bool HasThreat { get; set; }
        public float TimeSinceLastAction { get; set; }
        public int NeighborCount { get; set; }
        public float AverageNeighborDistance { get; set; }
        public bool IsInHerd { get; set; }
        public Vector3 CurrentVelocity { get; set; }
        public float CurrentSpeed { get; set; }
        public float Energy { get; set; } // Could represent sheep's current energy level
        public float Stress { get; set; } // Could represent accumulated stress
        public List<Transform> Threats { get; } = new();
        public Dictionary<Transform, float> ThreatRadius { get; } = new();
    }
}