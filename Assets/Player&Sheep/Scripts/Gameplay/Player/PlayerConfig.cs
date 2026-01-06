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
        [SerializeField] private float walkSpeed = 5f;
        [SerializeField] private float runSpeed = 8f;
        [SerializeField] private float rotationSpeed = 360f;

        [Header("Physics")]
        [SerializeField] private float gravity = -9.81f;

        [Header("Health")]
        [SerializeField, Tooltip("Maximum health of the player.")]
        private float maxHealth = 100f;
        
        [SerializeField, Tooltip("Starting health of the player.")]
        private float currentHealth = 100f;

        [SerializeField, Tooltip("Whether the player can die when health reaches 0.")]
        private bool canDie = true;

        [SerializeField, Tooltip("Whether the player can take damage.")]
        private bool canTakeDamage = true;

        [SerializeField, Tooltip("Whether the player can be healed.")]
        private bool canBeHealed = true;


        /// <summary>
        /// Invokes when any value changed.
        /// </summary>
        public event UnityAction<PlayerConfig> OnValueChanged;

        // Movement
        public float WalkSpeed => walkSpeed;
        public float RunSpeed => runSpeed;
        public float RotationSpeed => rotationSpeed;
        public float Gravity => gravity;

        // Health
        public float MaxHealth => maxHealth;
        public float CurrentHealth => currentHealth;
        public bool CanDie => canDie;
        public bool CanTakeDamage => canTakeDamage;
        public bool CanBeHealed => canBeHealed;


        private void OnValidate()
        {
            OnValueChanged?.Invoke(this);
        }
    }
}