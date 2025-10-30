using System.Collections;
using System.Collections.Generic;

using Gameplay.HealthSystem;

using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField, Tooltip("Bullet speed.")] private float speed = 50f;
    [SerializeField, Tooltip("Time before bullet despawning.")] private float lifeTime = 5f;


    private Rigidbody rb;
    private float damage;
    private Queue<Bullet> bulletPool;


    /// <summary>
    /// Initialization method.
    /// </summary>
    public void Initialize(float damage, Queue<Bullet> bulletPool)
    {
        rb = GetComponent<Rigidbody>();
        this.damage = damage;
        this.bulletPool = bulletPool;
    }


    public void Shoot(Vector3 direction)
    {
        rb.linearVelocity = direction * speed;
        StartCoroutine(DespawnCoroutine());
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<IDamageable>(out IDamageable _damageable))
        {
            _damageable.TakeDamage(damage);
        }

        StopAllCoroutines();
        gameObject.SetActive(false);
        bulletPool.Enqueue(this);
    }


    private IEnumerator DespawnCoroutine()
    {
        yield return new WaitForSeconds(lifeTime);
        gameObject.SetActive(false);
        bulletPool.Enqueue(this);
    }
}