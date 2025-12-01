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
        [SerializeField, Tooltip("Speed of the dog when it is very close to player.")]
        private float minSpeed;

        [SerializeField, Tooltip("Speed of the dog when it is very far from player.")]
        private float maxSpeed;

        [SerializeField, Tooltip("Speed of the dog during commands.")]
        private float baseSpeed;

        [SerializeField, Tooltip("Rotation speed of the dog.")]
        private float rotationSpeed;

        [Space, Header("Distance")]
        [SerializeField, Tooltip("Max distance between player and dog, when dog does NOT need to move.")]
        private float distanceToPlayer;

        [SerializeField, Tooltip("Distance between player and dog, when dog's speed is min.")]
        private float slowDistance;

        [SerializeField, Tooltip("Distance between player and dog, when dog's speed is max.")]
        private float maxDistance;

        [Space, Header("Bark Settings")]
        [SerializeField, Tooltip("Max distance of scared by bark objects..")]
        private float maxBarkDistance = 10f;

        [SerializeField, Tooltip("Angle around the dog on which bark is applied.")]
        private float barkAngle = 90f;

        [SerializeField, Tooltip("Min time between barks.")]
        private float barkCooldown = 2f;

        [SerializeField, Tooltip("Physics layers of objects that can be scared.")]
        private LayerMask scareableMask;
        

        /// <summary>
        /// Invokes when any value is changed in Inspector.
        /// </summary>
        public event UnityAction<DogConfig> OnValueChanged;


        /// <summary>
        /// Speed of the dog when it is very close to player.
        /// </summary>
        public float MinSpeed => minSpeed;
        /// <summary>
        /// Speed of the dog when it is very far from player.
        /// </summary>
        public float MaxSpeed => maxSpeed;
        /// <summary>
        /// Speed of the dog during commands.
        /// </summary>
        public float BaseSpeed => baseSpeed;
        /// <summary>
        /// Rotation speed of the dog.
        /// </summary>
        public float RotationSpeed => rotationSpeed;

        /// <summary>
        /// Max distance between player and dog, when dog does NOT need to move.
        /// </summary>
        public float DistanceToPlayer => distanceToPlayer;
        /// <summary>
        /// Distance between player and dog, when dog's speed is min.
        /// </summary>
        public float SlowDistance => slowDistance;
        /// <summary>
        /// Distance between player and dog, when dog's speed is max.
        /// </summary>
        public float MaxDistance => maxDistance;

        /// <summary>
        /// Max distance of scared by bark objects.
        /// </summary>
        public float MaxBarkDistance => maxBarkDistance;
        /// <summary>
        /// Angle around the dog on which bark is applied.
        /// </summary>
        public float BarkAngle => barkAngle;
        /// <summary>
        /// Min time between barks.
        /// </summary>
        public float BarkCooldown => barkCooldown;
        /// <summary>
        /// Physics layers of objects that can be scared.
        /// </summary>
        public LayerMask ScareableMask => scareableMask;


        private void OnValidate()
        {
            OnValueChanged?.Invoke(this);
        }
    }
}