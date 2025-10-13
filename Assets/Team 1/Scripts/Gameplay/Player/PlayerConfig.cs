using UnityEngine;
using UnityEngine.Events;

namespace Gameplay.Player
{
    /// <summary>
    /// Config with all data related to controls and stats of the player.
    /// </summary>
    [CreateAssetMenu(fileName = "PlayerConfig", menuName = "Configs/PlayerConfig")]
    public class PlayerConfig : ScriptableObject
    {
        [Header("Speed")]
        [SerializeField] private float _walkSpeed = 5f;
        [SerializeField] private float _runSpeed = 8f;
        [SerializeField] private float _rotationSpeed = 360f;

        [Header("Physics")]
        [SerializeField] private float _gravity = -9.81f;

        [Header("Health")]
        [Tooltip("Maximum health of the player.")]
        [SerializeField] private float _maxHealth = 100f;
        
        [Tooltip("Starting health of the player.")]
        [SerializeField] private float _currentHealth = 100f;

        [Tooltip("Whether the player can die when health reaches 0.")]
        [SerializeField] private bool _canDie = true;

        [Tooltip("Whether the player can take damage.")]
        [SerializeField] private bool _canTakeDamage = true;

        [Tooltip("Whether the player can be healed.")]
        [SerializeField] private bool _canBeHealed = true;


        public event UnityAction<PlayerConfig> OnValueChanged;

        // Movement
        public float WalkSpeed => _walkSpeed;
        public float RunSpeed => _runSpeed;
        public float RotationSpeed => _rotationSpeed;
        public float Gravity => _gravity;

        // Health
        public float MaxHealth => _maxHealth;
        public float CurrentHealth => _currentHealth;
        public bool CanDie => _canDie;
        public bool CanTakeDamage => _canTakeDamage;
        public bool CanBeHealed => _canBeHealed;

        private void OnValidate()
        {
            OnValueChanged?.Invoke(this);
        }
    }
}
