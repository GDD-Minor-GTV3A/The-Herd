using UnityEngine;

namespace Core.AI.Sheep.Config
{
    /// <summary>
    /// Tune for movement and flocking for sheeps
    /// </summary>
    [CreateAssetMenu(fileName = "SheepConfig", menuName = "Scriptable Objects/SheepConfig")]
    public class SheepConfig : ScriptableObject
    {
        [Header("Navigation and Movement")]
        [SerializeField] private float _baseSpeed = 2.2f;
        [SerializeField] private float _tick = 0.15f;

        [Header("Herding")]
        [SerializeField] private float _neighbourRadius = 2.0f;
        [SerializeField] private float _separationDistance = 0.8f;
        [SerializeField] private float _separationWeight = 1.2f;
        [SerializeField] private float _alignmentWeight = 0.6f;
        [SerializeField] private float _steerClamp = 2.5f;
        [SerializeField] private float _walkAwayFromHerdTicks = 2f; // This is a test timing and is open to change
        [SerializeField] private float _maxLostDistanceFromHerd = 10f; // This is a test distance and is open to change



        //Getters
        public float BaseSpeed => _baseSpeed;
        public float Tick => _tick;
        public float NeighborRadius => _neighbourRadius;
        public float SeparationDistance => _separationDistance;
        public float SeparationWeight => _separationWeight;
        public float AlignmentWeight => _alignmentWeight;
        public float SteerClamp => _steerClamp;
        public float WalkAwayFromHerdTicks => _walkAwayFromHerdTicks;
        public float MaxLostDistanceFromHerd => _maxLostDistanceFromHerd;
    }
}
