using Codice.Client.Common.GameUI;

using UnityEngine;
using UnityEngine.Events;

namespace Gameplay.Dog 
{
    /// <summary>
    /// Config with all data related to controlls of the dog.
    /// </summary>
    [CreateAssetMenu(fileName = "DogConfig", menuName = "Configs/DogConfig")]
    public class DogConfig : ScriptableObject
    {
        [Header("Speed")]
        [Tooltip("Speed of the dog when it is very close to player.")]
        [SerializeField] private float _minSpeed;
        [Tooltip("Speed of the dog when it is very far from player.")]
        [SerializeField] private float _maxSpeed;
        [Tooltip("Speed of the dog during commands.")]
        [SerializeField] private float _baseSpeed;
        [Tooltip("Rotation speed of the dog.")]
        [SerializeField] private float _rotationSpeed;

        [Space]
        [Header("Distance")]
        [Tooltip("Max distance between player and dog, when dog does NOT need to move.")]
        [SerializeField] private float _distanceToPlayer;
        [Tooltip("Distance between player and dog, when dog's speed is min.")]
        [SerializeField] private float _slowDistance;
        [Tooltip("Distance between player and dog, when dog's speed is max.")]
        [SerializeField] private float _maxDistance;


        public event UnityAction<DogConfig> OnValueChanged;


        /// <summary>
        /// Speed of the dog when it is very close to player.
        /// </summary>
        public float MinSpeed => _minSpeed;
        /// <summary>
        /// Speed of the dog when it is very far from player.
        /// </summary>
        public float MaxSpeed => _maxSpeed;
        /// <summary>
        /// Speed of the dog during commands.
        /// </summary>
        public float BaseSpeed => _baseSpeed;
        /// <summary>
        /// Rotation speed of the dog.
        /// </summary>
        public float RotationSpeed => _rotationSpeed;

        /// <summary>
        /// Max distance between player and dog, when dog does NOT need to move.
        /// </summary>
        public float DistanceToPlayer => _distanceToPlayer;
        /// <summary>
        /// Distance between player and dog, when dog's speed is min.
        /// </summary>
        public float SlowDistance => _slowDistance;
        /// <summary>
        /// Distance between player and dog, when dog's speed is max.
        /// </summary>
        public float MaxDistance => _maxDistance;


        private void OnValidate()
        {
            OnValueChanged?.Invoke(this);
        }
    }
}