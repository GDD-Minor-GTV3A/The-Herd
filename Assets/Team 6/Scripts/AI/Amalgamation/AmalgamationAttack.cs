using UnityEngine;
using UnityEngine.AI;
using System.Collections;

namespace Game.Scripts.Gameplay.Amalgamation
{
    /// <summary>
    /// Your line-attack weapon. No AI decisions. The state machine tells it when to fire.
    /// </summary>
    public class AmalgamationAttack : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform _firePoint;
        [SerializeField] private GameObject _bulletPrefab;

        [Header("Attack Settings")]
        [SerializeField] private float _attackCooldown = 3f;
        [SerializeField] private float _telegraphTime = 1f;
        [SerializeField] private int _bulletsInLine = 8;
        [SerializeField] private float _bulletSpacing = 1f;
        [SerializeField] private float _indicatorLength = 15f;
        [SerializeField] private float _rotationSpeed = 8f;

        // Filled by SecondAttack
        private Transform _player;
        private NavMeshAgent _agent;

        private bool _isAttacking;
        private float _nextAttackTime;

        public bool IsAttacking => _isAttacking;
        public bool IsOnCooldown => Time.time < _nextAttackTime;

        public void Initialize(Transform player, NavMeshAgent agent)
        {
            _player = player;
            _agent = agent;
        }

        public bool CanStartAttack()
        {
            return !_isAttacking && Time.time >= _nextAttackTime;
        }

        /// <summary>
        /// Called by the attack state (through AmalgamationSecondAttack).
        /// runner is the state machine MonoBehaviour.
        /// </summary>
        public Coroutine StartAttack(MonoBehaviour runner)
        {
            if (!CanStartAttack())
                return null;

            return runner.StartCoroutine(AttackRoutine());
        }

        private IEnumerator AttackRoutine()
        {
            _isAttacking = true;

            if (_agent != null)
            {
                _agent.isStopped = true;
                _agent.velocity = Vector3.zero;
            }

            float elapsed = 0f;

            // Telegraph: aim + debug ray
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

            // Fire the actual line
            FireBulletLine();

            _nextAttackTime = Time.time + _attackCooldown;
            _isAttacking = false;

            if (_agent != null)
            {
                _agent.isStopped = false;
            }
        }

        private void AimAtPlayer()
        {
            if (_player == null)
                return;

            Vector3 dir = _player.position - transform.position;
            dir.y = 0f;

            if (dir.sqrMagnitude < 0.001f)
                return;

            Quaternion targetRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRot,
                _rotationSpeed * Time.deltaTime
            );
        }

        private void FireBulletLine()
        {
            if (_bulletPrefab == null || _firePoint == null || _bulletsInLine <= 0)
                return;

            Vector3 right = _firePoint.right;
            float halfIndex = (_bulletsInLine - 1) * 0.5f;

            for (int i = 0; i < _bulletsInLine; i++)
            {
                float offset = (i - halfIndex) * _bulletSpacing;
                Vector3 spawnPos = _firePoint.position + right * offset;

                Instantiate(_bulletPrefab, spawnPos, _firePoint.rotation);
            }
        }
    }
}
