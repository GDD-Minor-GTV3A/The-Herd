using System.Collections;

using Gameplay.HealthSystem;

using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 50f;
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
