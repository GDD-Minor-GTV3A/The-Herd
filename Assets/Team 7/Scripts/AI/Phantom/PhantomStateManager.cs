using System;
using System.Collections.Generic;
using Core.Shared.StateMachine;
using Core.Shared.Utilities;

using Gameplay.Dog;
using Gameplay.HealthSystem;

using Team_7.Scripts.AI.Phantom.States;

using Unity.VisualScripting;

using UnityEngine;
using UnityEngine.Events;

using IState = Core.Shared.StateMachine.IState;
using Random = UnityEngine.Random;

namespace Team_7.Scripts.AI.Phantom
{
    public class PhantomStateManager : CharacterStateManager<IState>, IScareable, IDamageable
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
        private float _lastCloneSpawn;
        private List<PhantomFake> _clones = new();

        public UnityEvent DamageEvent { get; set; }
        
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
            
            //TODO Do this in a different way
            if (stats.damage > 0 )
                SpawnClones(stats.initialCloneAmount);
            
            SetState<WanderingState>();
        }

        protected override void Update()
        {
            base.Update();

            if (!GetComponent<PhantomFake>())
            {
                if (_lastCloneSpawn + stats.cloneSpawnDelay < Time.time &&
                    _clones.Count < stats.maxCloneAmount)
                {
                    SpawnClones(1);
                }
            }
            
            //TODO move this logic somewhere else(PhantomClone.cs)
            // Only make the enemy take damage from being looked at if stunned and a clone
            /*if (stats.damage <= 0 || _currentState is not StunnedState)
                return;
            
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
            }*/
        }

        protected override void InitializeStatesMap()
        {
            StatesMap = new Dictionary<Type, IState>
            {
                { typeof(WanderingState), new WanderingState(this, _enemyMovementController, stats, _phantomAnimatorController, _audioController) },
                { typeof(ShootingState), new ShootingState(this, _enemyMovementController, stats, _phantomAnimatorController, _audioController) },
                { typeof(StunnedState), new StunnedState(this, _enemyMovementController, stats, _phantomAnimatorController, _audioController) },
            };
        }

        private void Start()
        {
            Initialize();
        }

        public PhantomProjectile StartCharging()
        {
            _chargingProjectile = Instantiate(stats.projectile, projectileSpawn.transform.position, transform.rotation, transform);
            _chargingProjectile.Init(stats.chargeDuration, stats.projectileSpeed, stats.maxProjectileScale, stats.projectileRange, stats.damage, _playerObject, stats.homingStrength);
            _audioController.PlayClip(stats.projectileChargeSound);
            
            return _chargingProjectile;
        }

        public void CancelCharging()
        {
            if (_chargingProjectile is not null && !_chargingProjectile.IsDestroyed())
            {
                if (!_chargingProjectile.IsLaunched())
                    Destroy(_chargingProjectile.gameObject);
            }
            _audioController.StopClip();
            _chargingProjectile = null;
        }

        private void Respawn()
        {
            DestroyClones();
            transform.position += GenerateRandomSpawn();

            //TODO Do this in a different way
            if (stats.damage > 0 )
                SpawnClones(stats.initialCloneAmount);

            SetState<WanderingState>();
        }

        private Vector3 GenerateRandomSpawn()
        {
            // Pick a random direction on the unit sphere
            Vector3 randomDirection = Random.onUnitSphere;

            // Pick a random distance between min and max
            float randomDistance = Random.Range(stats.minRespawnDistance, stats.maxRespawnDistance);
            
            return randomDirection * randomDistance;
        }

        private void OnCollisionEnter(Collision other)
        {
            if (stats.damage != 0)
                return;
            
            if (other.gameObject.CompareTag("Player") && _currentState is StunnedState)
            {
                _currentHealth -= 1;
                if (_currentHealth > 0)
                    Respawn();
                else
                {
                    DestroyClones();
                    _audioController.PlayClip(stats.screechSound);
                    Destroy(gameObject);
                }
            }
        }
        
        private void OnTriggerEnter(Collider other)
        {
            // TODO Make the bullet collide with the Phantom instead of having to do it like this
            if (other.TryGetComponent<Bullet>(out var bullet))
            {
                TakeDamage(1);
                return;
            }
            
            if (stats.damage != 0)
                return;
            
            if (other.gameObject.CompareTag("Player") && _currentState is StunnedState)
            {
                _currentHealth -= 1;
                if (_currentHealth > 0)
                    Respawn();
                else
                {
                    DestroyClones();
                    _audioController.PlayClip(stats.screechSound);
                    Destroy(gameObject);
                }
            }
        }

        public void SpawnClones(int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                var clone = Instantiate(stats.fake, transform.position, transform.rotation);
                _clones.Add(clone);
                clone.transform.position += GenerateRandomSpawn();
            }

            _lastCloneSpawn = Time.time;
        }

        public void DestroyClones()
        {
            _clones.RemoveAll(clone => clone == null);
            foreach (var clone in _clones)
            {
                if (!clone.gameObject.IsDestroyed()){
                {
                    Destroy(clone.gameObject);
                }}
            }
            _clones.Clear();
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

        public void OnScared(Vector3 fromPosition, float intensity, ScareType scareType)
        {
            if (_currentState is not StunnedState)
                SetState<StunnedState>();
        }
        
        public void TakeDamage(float damage)
        {
            if (_currentState is not StunnedState)
                return;
            
            _currentHealth -= 1;
            if (_currentHealth > 0)
                Respawn();
            else
            {
                DestroyClones();
                _audioController.PlayClip(stats.screechSound);
                Destroy(gameObject);
            }
        }

        public void StartWandering()
        {
            SetState<WanderingState>();
        }
    }
}
