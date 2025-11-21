using UnityEngine;
using UnityEngine.AI;
using System.Collections;
namespace Game.Scripts.Gameplay.Amalgamation
{ 
 
    public class AmalgamationAttack : MonoBehaviour
    {
        #region Serialized Fields

        [Header("References")]
        [SerializeField]
        private Transform _player;

        [SerializeField]
        private NavMeshAgent _agent;

        [SerializeField]
        private Transform _firePoint;

        [SerializeField]
        private GameObject _bulletPrefab;

        [Header("Chase Settings")]
        [SerializeField]
        private float _chaseRange = 20f;

        [SerializeField]
        private float _attackRange = 10f;

        [SerializeField]
        private float _rotationSpeed = 8f;

        [Header("Attack Settings")]
        [SerializeField]
        private float _attackCooldown = 3f;

        [SerializeField]
        private float _telegraphTime = 1f;

        [SerializeField]
        private int _bulletsInLine = 8;

        [SerializeField]
        private float _bulletSpacing = 1f;

        [SerializeField]
        private float _indicatorLength = 15f;

        #endregion


        #region Private Fields

        private bool _isAttacking;
        private float _nextAttackTime;

        #endregion


        #region Unity Methods

        private void Update()
        {
            if (_player == null || _agent == null)
            {
                return;
            }

            if (_isAttacking)
            {
                return;
            }

            float distanceToPlayer = Vector3.Distance(transform.position, _player.position);

            if (distanceToPlayer > _chaseRange)
            {
                _agent.isStopped = true;
                return;
            }

            _agent.isStopped = false;
            _agent.SetDestination(_player.position);

            if (distanceToPlayer <= _attackRange && Time.time >= _nextAttackTime)
            {
                StartCoroutine(LineAttackCoroutine());
            }
        }

        #endregion


        #region Public Methods

       
        public void SetPlayerTarget(Transform playerTransform)
        {
            _player = playerTransform;
        }

        #endregion


        #region Private Methods

        private IEnumerator LineAttackCoroutine()
        {
            _isAttacking = true;
            _agent.isStopped = true;
            _agent.velocity = Vector3.zero;

            float elapsed = 0f;

        
            while (elapsed < _telegraphTime)
            {
                AimAtPlayer();

                if (_firePoint != null)
                {
                    Debug.DrawRay(
                        _firePoint.position,
                        _firePoint.forward * _indicatorLength,
                        Color.red
                    );
                }

                elapsed += Time.deltaTime;
                yield return null;
            }

            FireBulletLine();

            _nextAttackTime = Time.time + _attackCooldown;
            _isAttacking = false;
            _agent.isStopped = false;
        }


        private void AimAtPlayer()
        {
            if (_player == null)
            {
                return;
            }

            Vector3 direction = _player.position - transform.position;
            direction.y = 0f;

            if (direction.sqrMagnitude < 0.001f)
            {
                return;
            }

            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                _rotationSpeed * Time.deltaTime
            );
        }


        private void FireBulletLine()
        {
            if (_bulletPrefab == null || _firePoint == null || _bulletsInLine <= 0)
            {
                return;
            }

            Vector3 right = _firePoint.right;

           
            float halfIndex = (_bulletsInLine - 1) * 0.5f;

            for (int i = 0; i < _bulletsInLine; i++)
            {
                float offset = (i - halfIndex) * _bulletSpacing;
                Vector3 spawnPosition = _firePoint.position + right * offset;

                Instantiate(
                    _bulletPrefab,
                    spawnPosition,
                    _firePoint.rotation
                );
            }
        }

        #endregion
    }
}