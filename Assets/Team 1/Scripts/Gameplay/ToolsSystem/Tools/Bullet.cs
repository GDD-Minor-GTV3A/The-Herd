using System.Collections;

using Gameplay.HealthSystem;

using UnityEngine;
using UnityEngine.Pool;

public class Bullet : MonoBehaviour
{
    [SerializeField, Tooltip("Bullet speed.")] private float speed = 50f;
    [SerializeField, Tooltip("Time before bullet despawning.")] private float lifeTime = 5f;

    private Rigidbody rb;
    private float damage;
    private IObjectPool<Bullet> pool;

    public void Initialize(float damage, IObjectPool<Bullet> pool)
    {
        rb = GetComponent<Rigidbody>();
        this.damage = damage;
        this.pool = pool;
    }

    public void Shoot(Vector3 direction)
    {
        rb.linearVelocity = direction * speed;
        StartCoroutine(DespawnCoroutine());
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<IDamageable>(out IDamageable damageable))
            damageable.TakeDamage(damage);

        StopAllCoroutines();
        pool.Release(this);
    }

    private IEnumerator DespawnCoroutine()
    {
        yield return new WaitForSeconds(lifeTime);
        pool.Release(this);
    }
}
