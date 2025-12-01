using Codice.Client.BaseCommands.CheckIn;

using CustomEditor.Attributes;

using UnityEngine;
using UnityEngine.Events;

namespace Gameplay.ToolsSystem.Tools.Rifle
{
    /// <summary>
    /// Config for rifle tool.
    /// </summary>
    [CreateAssetMenu(fileName = "RifleConfig", menuName = "Scriptable Objects/RifleConfig")]
    public class RifleConfig : ScriptableObject
    {
        [Header("Ammo")]
        [SerializeField, Tooltip("Maximum number of rounds the rifle can hold at once.")]
        private int magazineCapacity = 5;
        [SerializeField, Tooltip("How many free bullets player will have from the beginning.")]
        private uint initialFreeAmmos = 5;

        [Header("FireSpread")]
        [SerializeField, Tooltip("Angle of fire spread.")]
        private float spreadAngle = 10;

        [Header("Timing")]
        [SerializeField, Tooltip("Total time (in seconds) to fully reload the rifle.")]
        private float reloadTime = 5f;

        [SerializeField, Tooltip("Total duration (in seconds) for a complete bolt cycle (open, eject, close).")]
        private float boltCycleTime = 1.5f;

        [Header("Damage")]
        [SerializeField, Tooltip("Damage dealt per bullet fired.")]
        private float damage = 25f;

        [Header("Bullet")]
        [SerializeField, Tooltip("Bullet prefab reference.")]
        private Bullet bulletPrefab;

        [Header("Pool Settings")]
        [SerializeField, Tooltip("Maximum number of pooled bullets in memory.")]
        private int maxPoolSize = 50;

        [Header("For tests")]
        [SerializeField]
        private bool infiniteAmmo = false;
        [SerializeField, ShowIf("infiniteAmmo")]
        private bool infiniteMagazine = false;


        /// <summary>
        /// Event fired when any config value changes (optional)
        /// </summary>
        public event UnityAction<RifleConfig> OnValueChanged;


        // --- Read-only public properties ---
        public int MaxAmmo => magazineCapacity;
        public uint InitialFreeAmmo => initialFreeAmmos;
        public float SpreadAngle => spreadAngle;
        public float ReloadTime => reloadTime;
        public float BoltCycleTime => boltCycleTime;
        public float Damage => damage;
        public Bullet BulletPrefab => bulletPrefab;
        public int MaxPoolSize => maxPoolSize;
        public bool IsInfiniteAmmo => infiniteAmmo;
        public bool IsInfiniteMagazine => infiniteMagazine;


        private void OnValidate()
        {
            OnValueChanged?.Invoke(this);
        }
    }
}