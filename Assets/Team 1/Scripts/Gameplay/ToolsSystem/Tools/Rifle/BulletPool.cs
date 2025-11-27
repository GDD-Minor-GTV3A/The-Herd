using Core.Shared;
using UnityEngine;

namespace Gameplay.ToolsSystem.Tools.Rifle
{
    /// <summary>
    /// Concrete Bullet pool using GenericPool
    /// </summary>
    public class BulletPool : GenericPool<Bullet>
    {
        private readonly Bullet bulletPrefab;
        private readonly float bulletDamage;


        /// <param name="prefab">Prefab of bullet object.</param>
        /// <param name="damage">Damage of hte bullet.</param>
        /// <param name="initialCapacity">Initial amount of bullets in pool.</param>
        /// <param name="maxSize"></param>
        public BulletPool(Bullet prefab, float damage, int initialCapacity = 0, int maxSize = 50)
            : base(initialCapacity, maxSize)
        {
            bulletPrefab = prefab;
            bulletDamage = damage;
        }


        protected override Bullet Create()
        {
            Bullet bullet = Object.Instantiate(bulletPrefab);
            bullet.Initialize(bulletDamage, Release); // pass callback to release back to pool
            bullet.gameObject.SetActive(false);
            return bullet;
        }

        protected override void OnGet(Bullet bullet)
        {
            bullet.gameObject.SetActive(true);
        }

        protected override void OnRelease(Bullet bullet)
        {
            bullet.gameObject.SetActive(false);
        }

        protected override void OnDestroy(Bullet bullet)
        {
            GameObject.Destroy(bullet.gameObject);
        }
    }
}