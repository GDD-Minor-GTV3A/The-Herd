using System.Collections;
using Gameplay.HealthSystem;
using UnityEngine;

namespace Gameplay.ToolsSystem.Tools.Rifle
{
    /// <summary>
    /// Controls bullet logic.
    /// </summary>
    public class Bullet : MonoBehaviour
    {
        [Header("Bullet Settings")]
        [SerializeField, Tooltip("Speed at which the bullet travels after being fired.")]
        private float speed = 50f;

        [SerializeField, Tooltip("Time in seconds before the bullet is automatically returned to the pool if it doesn't hit anything.")]
        private float lifeTime = 5f;


        private Rigidbody rb;
        private float damage;
        private System.Action<Bullet> releaseCallback;


        /// <summary>
        /// Initialization method.
        /// </summary>
        /// <param name="damage">Damage of the bullet.</param>
        /// <param name="releaseCallback">Action to get bullet back to pool.</param>
        public void Initialize(float damage, System.Action<Bullet> releaseCallback)
        {
            rb = GetComponent<Rigidbody>();
            this.damage = damage;
            this.releaseCallback = releaseCallback;
        }


        /// <summary>
        /// Shoot hte bullet.
        /// </summary>
        /// <param name="direction">Direction of shot.</param>
        public void Shoot(Vector3 direction)
        {
            transform.forward = direction;
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
}