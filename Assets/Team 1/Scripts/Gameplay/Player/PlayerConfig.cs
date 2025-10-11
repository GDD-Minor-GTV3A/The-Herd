using UnityEngine;
using UnityEngine.Events;
namespace Gameplay.Player
{
    /// <summary>
    /// Config with all data related to controlls of the player.
    /// </summary>
    [CreateAssetMenu(fileName = "PlayerConfig", menuName = "Configs/PlayerConfig")]
    public class PlayerConfig : ScriptableObject
    {
        [Header("Speed")]
        [Tooltip("General speed of the player.")]
        [SerializeField] private float _walkSpeed;
        [Tooltip("Sprint speed of the player.")]
        [SerializeField] private float _runSpeed;
        [Tooltip("Rotation speed of the player.")]
        [SerializeField] private float _rotationSpeed;

        [Space]
        [Tooltip("Gravity applied to the player.")]
        [SerializeField] private float _gravity;


        public event UnityAction<PlayerConfig> OnValueChanged;


        /// <summary>
        /// General speed of the player.
        /// </summary>
        public float WalkSpeed => _walkSpeed;
        /// <summary>
        /// Sprint speed of the player.
        /// </summary>
        public float RunSpeed => _runSpeed;
        /// <summary>
        /// Rotation speed of the player.
        /// </summary>
        public float RotationSpeed => _rotationSpeed;

        /// <summary>
        /// Gravity applied to the player.
        /// </summary>
        public float Gravity => _gravity;


        private void OnValidate()
        {
            OnValueChanged?.Invoke(this);
        }
    }
}

