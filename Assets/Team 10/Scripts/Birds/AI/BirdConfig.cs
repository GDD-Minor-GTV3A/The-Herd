using UnityEngine;

namespace Birds.AI
{
    [CreateAssetMenu(fileName = "BirdConfig", menuName = "Birds/BirdConfig")]
    public class BirdConfig : ScriptableObject
    {
        [Header("Movement Settings")]
        [Tooltip("How fast the birds fly away")]
        [SerializeField] private float flightSpeed = 10f;

        [Tooltip("How high the birds fly when fleeing")]
        [SerializeField] private float flightHeight = 15f;
        
        [Header("Timing")]
        [Tooltip("Time in seconds before birds return after disappearing")]
        [SerializeField] private float returnDelay = 5f;

        [Tooltip("Distance from original spot to spawn when returning")]
        [SerializeField] private float spawnRadius = 20f;

        // Public Getters
        public float FlightSpeed => flightSpeed;
        public float FlightHeight => flightHeight;
        public float ReturnDelay => returnDelay;
        public float SpawnRadius => spawnRadius;
    }
}