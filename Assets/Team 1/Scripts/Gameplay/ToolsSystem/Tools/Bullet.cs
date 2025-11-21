using System.Collections;

using Core.Shared.Utilities;

using Gameplay.HealthSystem;

using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Bullet Settings")]

    [Tooltip("Speed at which the bullet travels after being fired.")]
    [SerializeField] private float speed = 50f;

    [Tooltip("Time in seconds before the bullet is automatically returned to the pool if it doesn't hit anything.")]
    [SerializeField] private float lifeTime = 5f;

    private Rigidbody rb;
    private float damage;
    private System.Action<Bullet> releaseCallback;

    public void Initialize(float damage, System.Action<Bullet> releaseCallback)
    {
        rb = GetComponent<Rigidbody>();
        this.damage = damage;
        this.releaseCallback = releaseCallback;
    }

    public void Shoot(Vector3 direction)
    {
        rb.linearVelocity = direction * speed;
        StartCoroutine(DespawnCoroutine());
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<IDamageable>(out var damageable))
            damageable.TakeDamage(damage);

        StopAllCoroutines();
        releaseCallback?.Invoke(this);
    }

    private IEnumerator DespawnCoroutine()
    {
        yield return new WaitForSeconds(lifeTime);
        releaseCallback?.Invoke(this);
    }
}
