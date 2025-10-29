using System;
using System.Collections.Generic;
using UnityEngine;
using Core;
using Core.Shared.StateMachine;
using Phantom;

using Random = UnityEngine.Random;
using AI.Phantom.States;
using Core.Shared.Utilities;

namespace AI.Phantom
{
    public class PhantomStateManager : CharacterStateManager<IState>
    {
        [SerializeField][Required] private PhantomStats stats;
        [SerializeField] private GameObject projectileSpawn;
        private EnemyMovementController _enemyMovementController;
        private PhantomAnimatorController _phantomAnimatorController;
        private AudioController _audioController;
        private float _currentHealth;
        private PhantomProjectile _chargingProjectile;
        private Transform _playerTransform;
        private GameObject _playerObject;
        private float _startedLooking;

        public void Initialize()
        {
            // Get the movement controller
            _enemyMovementController = gameObject.GetComponent<EnemyMovementController>();
            if (_enemyMovementController is null)
            {
                Debug.LogWarning("A Phantom enemy is missing it's EnemyMovementController component, a new EnemyMovementController has been created");
                _enemyMovementController = gameObject.AddComponent<EnemyMovementController>();
            }
            _movementController = _enemyMovementController;
            
            // Get the animator controller
            var animatorControllerComponent = GetComponentInChildren<Animator>();
            Animator animator = GetComponentInChildren<Animator>();
            if (animatorControllerComponent is null)
            {
                Debug.LogWarning("A Phantom enemy is missing it's Animator component, a new Animator has been created");
                animator = gameObject.AddComponent<Animator>();
            }
            _phantomAnimatorController = _phantomAnimatorController = new PhantomAnimatorController(animator);
            _animatorController = _phantomAnimatorController;

            // IMP01 - Samuele
            var _audioSource = GetComponent<AudioSource>();
            _audioController = new AudioController(_audioSource);
            
            InitializeStatesMap();
            
            //TODO replace this
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                _playerTransform = playerObj.transform;
                _playerObject = playerObj;
            }
            else
            {
                Debug.LogError("EnemyAI: No object with tag 'Player' found!");
                enabled = false;
                return;
            }

            _currentHealth = stats.health;
            SetState<WanderingState>();
        }

        protected override void Update()
        {
            base.Update();
            _playerTransform = _playerObject.transform;

            var playerTransform = _playerTransform;
            var directionToPlayer = transform.position - playerTransform.position;
            var playerLookDirection = playerTransform.forward;
            
            var angle = Vector3.Angle(playerLookDirection, directionToPlayer);
            var distance = Vector3.Distance(playerTransform.position, transform.position);
            if (angle <= stats.DamageAngle && _startedLooking == 0 && distance < stats.damageDistance)
                _startedLooking = Time.time;
            else if (angle >= stats.DamageAngle || distance > stats.damageDistance)
                _startedLooking = 0;

            if (_startedLooking + stats.lookDuration < Time.time && _startedLooking != 0)
            {
                _startedLooking = 0;
                _currentHealth -= 1;

                if (_currentHealth > 0)
                    Respawn();
                else
                    Destroy(gameObject);
            }
        }

        protected override void InitializeStatesMap()
        {
            StatesMap = new Dictionary<Type, IState>
            {
                { typeof(WanderingState), new WanderingState(this, _enemyMovementController, stats, _phantomAnimatorController, _audioController) },
                { typeof(ShootingState), new ShootingState(this, _enemyMovementController, stats, _phantomAnimatorController, _audioController) },
            };
        }

        private void Start()
        {
            Initialize();
        }

        public PhantomProjectile StartCharging()
        {
            _chargingProjectile = Instantiate(stats.projectile, projectileSpawn.transform.position, transform.rotation, transform);
            _chargingProjectile.Init(stats.chargeDuration, stats.projectileSpeed, stats.maxProjectileScale, stats.projectileRange);

            return _chargingProjectile;
        }

        public void CancelCharging()
        {
            if (!_chargingProjectile.IsLaunched())
                Destroy(_chargingProjectile);
            
            _chargingProjectile = null;
        }

        private void Respawn()
        {
            // Pick a random direction on the unit sphere
            Vector3 randomDirection = Random.onUnitSphere;

            // Pick a random distance between min and max
            float randomDistance = Random.Range(stats.minRespawnDistance, stats.maxRespawnDistance);
            
            transform.position += randomDirection * randomDistance;
            SetState<WanderingState>();
        }

        //TODO replace the way the player is found
        public Transform GetPlayerTransform()
        {
            return _playerTransform;
        }

        public bool IsBeingLookedAt()
        {
            return _startedLooking != 0;
        }
    }
}
