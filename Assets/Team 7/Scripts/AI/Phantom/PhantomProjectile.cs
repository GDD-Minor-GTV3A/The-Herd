using Gameplay.HealthSystem;
using Gameplay.Player;

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
        private GameObject _target;
        private float _homingStrength;

        public void Init(float chargeDuration, float projectileSpeed, Vector3 maxProjectileScale, float projectileRange, int projectileDamage, GameObject player, float homingStrength)
        {
            _projectileSpeed = projectileSpeed;
            _projectileRange = projectileRange;
            _maxProjectileScale = maxProjectileScale;
            _chargeDuration = chargeDuration;
            _projectileDamage = projectileDamage;
            _chargeStart = Time.time;
            _target = player;
            _homingStrength = homingStrength;
        }

        // Update is called once per frame
        void Update()
        {
            if (!_launched)
            {
                gameObject.transform.localScale = _maxProjectileScale * Mathf.Clamp01((Time.time - _chargeStart) / _chargeDuration );
                return;
            }

            if (_target is not null)
            {
                var character = _target.GetComponent<CharacterController>();
                Vector3 targetDir = ((_target.transform.position + Vector3.up * (character.height / 2)) - transform.position).normalized;
                
                // Only home towards the player if flying towards them
                if (Vector3.Dot(_launchDirection, targetDir) > 0f)
                {
                    Vector3 adjustedDirection = targetDir.normalized;
                    _launchDirection = Vector3.Slerp(_launchDirection, adjustedDirection, _homingStrength * Time.deltaTime);
                    _launchDirection.Normalize();
                }
            }

            
            transform.position += _launchDirection * (_projectileSpeed * Time.deltaTime);
            
            // Destroy the projectile once it has travelled it's max distance
            float distanceSqr = (_launchLocation - transform.position).sqrMagnitude;
            if (distanceSqr >= _projectileRange * _projectileRange)
                Destroy(gameObject);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.TryGetComponent(out Player damageable))
            {
                damageable.TakeDamage(_projectileDamage);
                Destroy(gameObject);
            }
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
