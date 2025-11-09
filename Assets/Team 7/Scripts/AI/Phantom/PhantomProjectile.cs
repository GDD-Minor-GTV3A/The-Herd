using Gameplay.HealthSystem;

using UnityEngine;

namespace Team_7.Scripts.AI.Phantom
{
    public class PhantomProjectile : MonoBehaviour
    {
        private Vector3 _maxProjectileScale;
        private float _projectileSpeed;
        private float _projectileRange;
        private int _projectileDamage;
        private float _chargeDuration;
        private float _chargeStart;
        private Vector3 _launchDirection;
        private Vector3 _launchLocation;
        private bool _launched;

        public void Init(float chargeDuration, float projectileSpeed, Vector3 maxProjectileScale, float projectileRange, int projectileDamage)
        {
            _projectileSpeed = projectileSpeed;
            _projectileRange = projectileRange;
            _maxProjectileScale = maxProjectileScale;
            _chargeDuration = chargeDuration;
            _projectileDamage = projectileDamage;
            _chargeStart= Time.time;
        }

        // Update is called once per frame
        void Update()
        {
            gameObject.transform.localScale = _maxProjectileScale * Mathf.Clamp01((Time.time - _chargeStart) / _chargeDuration );

            if (!_launched)
            {
                return;
            }
            transform.position += _launchDirection * (_projectileSpeed * Time.deltaTime);
                
            // Destroy the projectile once it has travelled it's max distance
            float distanceSqr = (_launchLocation - transform.position).sqrMagnitude;
            if (distanceSqr >= _projectileRange * _projectileRange)
                Destroy(gameObject);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.gameObject.CompareTag("Player"))
                return;

            if (other.gameObject.TryGetComponent(out IDamageable _damageable))
            {
                _damageable.TakeDamage(_projectileDamage);
            }
            
            Destroy(gameObject);
        }

        public bool IsLaunched()
        {
            return _launched;
        }

        public void Launch()
        {
            _launchDirection = transform.parent.forward.normalized;
            _launchLocation = transform.position;
            transform.parent = null;
            _launched = true;
        }
    }
}
