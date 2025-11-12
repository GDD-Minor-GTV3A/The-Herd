using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "RifleConfig", menuName = "Scriptable Objects/RifleConfig")]
public class RifleConfig : ScriptableObject
{
    [Header("Ammo")]
    [Tooltip("Maximum number of rounds the rifle can hold at once.")]
    [SerializeField] private int _maxAmmo = 5;

    [Header("Timing")]
    [Tooltip("Total time (in seconds) to fully reload the rifle.")]
    [SerializeField] private float _reloadTime = 5f;

    [Tooltip("Total duration (in seconds) for a complete bolt cycle (open, eject, close).")]
    [SerializeField] private float _boltCycleTime = 1.5f;

    [Header("Damage")]
    [Tooltip("Damage dealt per bullet fired.")]
    [SerializeField] private float _damage = 25f;

    [Header("Bullet")]
    [Tooltip("Bullet prefab reference.")]
    [SerializeField] private Bullet _bulletPrefab;

    [Header("Pool Settings")]
    [Tooltip("Maximum number of pooled bullets in memory.")]
    [SerializeField] private int _maxPoolSize = 50;

    // Event fired when any config value changes (optional)
    public event UnityAction<RifleConfig> OnValueChanged;

    // --- Read-only public properties ---
    public int MaxAmmo => _maxAmmo;
    public float ReloadTime => _reloadTime;
    public float BoltCycleTime => _boltCycleTime;
    public float Damage => _damage;
    public Bullet BulletPrefab => _bulletPrefab;
    public int MaxPoolSize => _maxPoolSize;

    private void OnValidate()
    {
        OnValueChanged?.Invoke(this);
    }
}
