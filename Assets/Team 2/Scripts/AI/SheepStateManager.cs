using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using Core.Events;
using Core.Shared.StateMachine;
using Core.AI.Sheep.Config;
using Core.AI.Sheep.Event;
using Core.AI.Sheep.Personality;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

using Random = UnityEngine.Random;

namespace Core.AI.Sheep
{
    /// <summary>
    /// Sheep state manager
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent))]
    [DisallowMultipleComponent]
    public class SheepStateManager : StateManager<IState>
    {
        private const float DEFAULT_FOLLOW_DISTANCE = 1.8f;
        private const float DEFAULT_MAX_LOST_DISTANCE_FROM_HERD = 10f; // This is a test distance and is open to change
        private const float DEFAULT_WALK_AWAY_FROM_HERD_TICKS = 2f; // This is a test timing and is open to change

        [Header("Data")]
        [SerializeField]
        [Tooltip("Movement and herding config")]
        private SheepConfig _config;

        [SerializeField][Tooltip("Sheep's archetype")]
        private SheepArchetype _archetype;

        [SerializeField]
        private SheepAnimationDriver _animation;

        [SerializeField] private float _deathDistanceInterval = 0.5f;

        [Header("Sounds")]
        [SerializeField][Tooltip("Sheep sound driver")]
        private SheepSoundDriver _sheepSoundDriver;

        [Header("Neighbours")]
        [SerializeField]
        [Tooltip("All herd members")]
        private List<Transform> _neighbours = new List<Transform>();
        
        [Header("VFX")] [SerializeField] private GameObject _joinHerdVFXPrefab;
        [SerializeField] private GameObject _leaveHerdVFXPrefab;
        [SerializeField] private Vector3 _vfxOffset = new Vector3(0f, 0.3f, 0f);

        [Header("Petting")] [SerializeField] private Sprite _flashbackImage;
        
        private readonly Dictionary<Transform, float> _threats = new();
        private readonly Dictionary<Transform, float> _threatRadius = new();
        [SerializeField] private float _threatMemory = 1.25f;
        private Coroutine _panicLoop;
        private bool _hadThreatLastFrame;


        //Private
        private NavMeshAgent _agent;
        private Coroutine _tickCoroutine;
        private float _nextWalkingAwayFromHerdAt;

        private Vector3 _playerCenter;
        private Vector3 _playerHalfExtents;
        private static readonly List<SheepStateManager> _allSheep = new();
        private float _nextDeathDistanceCheck;
        private bool _diedByDistance;
        

        // Personality system
        private ISheepPersonality _personality;
        private PersonalityBehaviorContext _behaviorContext;

        [SerializeField] private bool _startAsStraggler;
        [SerializeField] private float _joinGrace = 3f;
        private float _walkAwayReenableAt;

        /// <summary>
        /// Exposed NavMeshAgent for state machine
        /// </summary>
        public NavMeshAgent Agent => _agent;

        public SheepAnimationDriver Animation => _animation;
        
        public bool IsStraggler => _startAsStraggler;
        
        public Sprite FlashbackImage => _flashbackImage;
        public Vector3 PlayerSquareCenter => _playerCenter;
        public Vector3 PlayerSquareHalfExtents => _playerHalfExtents;

        /// <summary>
        /// Exposed config and archetype and sound driver
        /// </summary>
        public SheepConfig Config => _config;
        public SheepArchetype Archetype => _archetype;
        public ISheepPersonality Personality => _personality;
        public SheepSoundDriver SoundDriver => _sheepSoundDriver;

        /// <summary>
        /// Read-only list of neighbouring sheep
        /// </summary>

        public static IReadOnlyList<SheepStateManager> AllSheep => _allSheep;
        public IReadOnlyList<Transform> Neighbours => _neighbours;

        public void MarkAsStraggler() => _startAsStraggler = true;

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            if(_config != null)
            {
                _agent.speed = _config.BaseSpeed;
            }

            if(_archetype?.AnimationOverrides != null)
            {
                _animation?.ApplyOverrideController(_archetype.AnimationOverrides);
            }
            
            _personality = _archetype?.CreatePersonality(this);
            _behaviorContext = new PersonalityBehaviorContext();
            _playerCenter = FindObjectOfType<SheepHerdController>().transform.position;

            InitializeStatesMap();
        }

        private void LateUpdate()
        {
            if(!_animation || !_agent) return;
            Vector3 v = _agent.velocity;
            v.y = 0f;
            _animation.SetSpeed(v.magnitude);
            // Debug.Log("Set speed to: "  + v.magnitude);    |   Chris: Commenting this out cuz it keeps sending log every frame which is annoying
        }

        private void OnEnable()
        {
            if (!_allSheep.Contains(this))
                _allSheep.Add(this);
            EnableBehavior();
            SetState<SheepGrazeState>();
        }

        private void OnDisable()
        {
            _allSheep.Remove(this);
            DisableBehavior();
        }

        private void OnDestroy()
        {
            _allSheep.Remove(this);
        }

        /// <summary>
        /// Enable event listeners and tick coroutine
        /// </summary>
        public void EnableBehavior()
        {
            EventManager.AddListener<PlayerSquareChangedEvent>(OnPlayerSquareChanged);
            EventManager.AddListener<PlayerSquareTickEvent>(OnPlayerSquareTick);
            EventManager.AddListener<RequestPetSheepEvent>(OnPetRequested);

            if (_tickCoroutine == null)
            {
                float interval = _config != null ? _config.Tick : 0.15f;
                _tickCoroutine = StartCoroutine(TickCoroutine(interval));
            }
        }

        /// <summary>
        /// Disable event listeners and tick coroutine
        /// </summary>
        public void DisableBehavior()
        {
            EventManager.RemoveListener<PlayerSquareChangedEvent>(OnPlayerSquareChanged);
            EventManager.RemoveListener<PlayerSquareTickEvent>(OnPlayerSquareTick);
            EventManager.RemoveListener<RequestPetSheepEvent>(OnPetRequested);
            
            if(_tickCoroutine != null)
            {
                StopCoroutine(_tickCoroutine);
                _tickCoroutine = null;
            }
        }

        protected override void InitializeStatesMap()
        {
            StatesMap = new Dictionary<Type, IState>
            {
                {typeof(SheepFollowState), new SheepFollowState(this)},
                {typeof(SheepGrazeState), new SheepGrazeState(this)},
                {typeof(SheepWalkAwayFromHerdState), new SheepWalkAwayFromHerdState(this)},
                {typeof(SheepFreezeState), new SheepFreezeState(this)},
                {typeof(SheepMoveState), new SheepMoveState(this)},
                {typeof(SheepPettingState), new SheepPettingState(this)},
                {typeof(SheepScaredState), new SheepScaredState(this)},
                {typeof(SheepDieState), new SheepDieState(this)},
            };
        }

        /// <summary>
        /// Fill list with neighbours
        /// </summary>
        public void SetNeighbours(List<Transform> neighbours)
        {
            _neighbours = neighbours ?? new List<Transform>();
        }

        private void RefreshNeighbours()
        {
            _neighbours.Clear();

            float radius = _config.NeighborRadius;
            float r2 = radius * radius;
            
            Vector3 p = transform.position;

            foreach (var s in _allSheep)
            {
                if (s == this || !s.isActiveAndEnabled)
                    continue;
                
                if ((s.transform.position - p).sqrMagnitude <= r2)
                    _neighbours.Add(s.transform);
            }
        }
        
        public void PlayJoinHerdVfx()
        {
            SpawnVFX(_joinHerdVFXPrefab);
        }

        public void PlayLeaveHerdVfx()
        {
            SpawnVFX(_leaveHerdVFXPrefab);
        }

        private void SpawnVFX(GameObject prefab)
        {
            if (!prefab) return;

            var vfx = Instantiate(
                prefab,
                transform.position + _vfxOffset,
                Quaternion.identity);
            
            vfx.transform.SetParent(transform, true);

            if (vfx.TryGetComponent<ParticleSystem>(out var ps))
            {
                var main = ps.main;
                float lifetime = main.duration + main.startLifetimeMultiplier;
                Destroy(vfx, lifetime);
            }
            else
            {
                Destroy(vfx, 2f);
            }
        }
        
        private void OnPlayerSquareChanged(PlayerSquareChangedEvent e)
        {
            _playerCenter = e.Center;
            _playerHalfExtents = e.HalfExtents;
        }

        private void OnPlayerSquareTick(PlayerSquareTickEvent e)
        {
            _playerCenter = e.Center;
            _playerHalfExtents = e.HalfExtents;

            if (_startAsStraggler) return;

            if (_currentState is SheepWalkAwayFromHerdState
                || _currentState is SheepScaredState
                || _currentState is SheepFreezeState
                || _currentState is SheepDieState
                || _currentState is SheepPettingState
                || _currentState is SheepMoveState) return;

            //Decide on state
            bool outside = FlockingUtility.IsOutSquare(transform.position, _playerCenter, _playerHalfExtents);
            
            // Redundant check, state machine should check if switching to same state
            Type targetState = outside ? typeof(SheepFollowState) : typeof(SheepGrazeState);
            if(_currentState.GetType() == targetState) return;
            
            if (outside)
                SetState<SheepFollowState>();
            else
                SetState<SheepGrazeState>();
        }

        private void OnSheepCallBackToPlayerEvent()
        {
            SetState<SheepFollowState>();
        }
        
        private IEnumerator TickCoroutine(float interval)
        {
            var wait = new WaitForSeconds(interval);

            while(true)
            {
                /*CheckDeathByDistance();
                if (_diedByDistance)
                {
                    yield break;
                }*/
                
                RefreshNeighbours();
                // Update behavior context for personality
                UpdateBehaviorContext();

                if (_currentState is not SheepWalkAwayFromHerdState
                    && _currentState is not SheepScaredState
                    && Time.time >= _nextWalkingAwayFromHerdAt
                    && Time.time >= _walkAwayReenableAt
                    && !_startAsStraggler)
                {
                    if (Random.value <= _archetype.GettingLostChance)
                        SetState<SheepWalkAwayFromHerdState>();

                    ScheduleNextWalkAwayFromHerd();
                }

                if (_sheepSoundDriver != null)
                {
                    if (_behaviorContext.CurrentVelocity != Vector3.zero)
                        _sheepSoundDriver.TryPlayWalkSound();

                    _sheepSoundDriver.TryPlayBleatSound(_archetype);
                }

                yield return wait;
            }
        }

        private void CheckDeathByDistance()
        {
            if (_diedByDistance) return;
            if (_config == null) return;
            if (_startAsStraggler) return;

            float killDist = _config.DeathDistance;
            if (killDist <= 0f) return;

            Vector3 delta = transform.position - _playerCenter;
            delta.y = 0f;

            if (delta.sqrMagnitude > killDist * killDist)
            {
                KillByDistance();
            }
        }

        private void KillByDistance()
        {
            if (_diedByDistance) return;
            _diedByDistance = true;

            var health = GetComponent<SheepHealth>();
            if (health != null)
            {
                EventManager.Broadcast(new SheepDamageEvent(
                    this,
                    health.MaxHealth,
                    transform.position,
                    SheepDamageType.DeathCircle,
                    gameObject));
            }
            else
            {
                Remove();
            }
        }

        public void ReportThreat(Transform enemy, Vector3 pos, float radius)
        {
            if (enemy == null) return;
            _behaviorContext.HasThreat = true;
            _behaviorContext.ThreatPosition = pos;
            _threats[enemy] = Time.time;
            _threatRadius[enemy] = radius;
        }

        public void ForgetThreat(Transform enemy)
        {
            if (enemy == null) return;
            _threats.Remove(enemy);
            _threatRadius.Remove(enemy);
            if(_threats.Count == 0)
            {
                _behaviorContext.HasThreat = false;
                _behaviorContext.ThreatPosition = Vector3.zero;
            }
        }

        private void StartPanicLoop()
        {
            if (_panicLoop != null) return;

            if (CanControlAgent())
            {
                Agent.autoRepath = true;
                Agent.acceleration = Mathf.Max(Agent.acceleration, (Config?.BaseSpeed ?? 2.2f));
                Agent.angularSpeed = Mathf.Max(Agent.angularSpeed, 1080f);
                Agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
            }

            _panicLoop = StartCoroutine(PanicSteerLoop());
        }

        private void StopPanicLoop()
        {
            if (_panicLoop == null) return;
            StopCoroutine(_panicLoop);
            _panicLoop = null;

            if (CanControlAgent())
            {
                Agent.updateRotation = true;
            }
        }

        private IEnumerator PanicSteerLoop()
        {
            var wait = new WaitForSeconds(0.03f);

            const float cooloff = 0.15f;
            float lastThreatSeenAt = Time.time;

            while (true)
            {
                if (_behaviorContext.HasThreat)
                {
                    lastThreatSeenAt = Time.time;
                }

                _personality.SetDestinationWithHerding(transform.position, this, _behaviorContext);

                if (!_behaviorContext.HasThreat && Time.time - lastThreatSeenAt > cooloff)
                {
                    break;
                }

                yield return wait;
            }

            _panicLoop = null;
        }

        private void OnPetRequested(RequestPetSheepEvent evt)
        {
            if (evt.TargetSheep != this) return;

            if (_currentState is SheepDieState || _currentState is SheepWalkAwayFromHerdState) return;
            
            string currentScene = SceneManager.GetActiveScene().name;
            if (_personality != null && !_personality.CanBePetted(currentScene))
            {
                Debug.Log($"{name} can't be petted in {currentScene}");
                return;
            }
            
            SetState<SheepPettingState>();
        }
        private void ScheduleNextWalkAwayFromHerd()
        {
            _nextWalkingAwayFromHerdAt = Time.time + _config?.WalkAwayFromHerdTicks ?? DEFAULT_WALK_AWAY_FROM_HERD_TICKS;
        }

        public bool IsCurrentlyOutsideHerd()
        {
            return FlockingUtility.IsOutSquare(transform.position, _playerCenter, _playerHalfExtents);
        }

        public void SummonToHerd(float? graceSeconds = null, bool clearThreats = true)
        {
            Debug.Log("Event");
            _startAsStraggler = false;

            // apply grace so it doesn't immediately get lost again
            float grace = graceSeconds ?? _joinGrace;
            _walkAwayReenableAt = Time.time + grace;
            ScheduleNextWalkAwayFromHerd();

            if (clearThreats)
            {
                StopPanicLoop();
                _threats.Clear();
                _threatRadius.Clear();
                _behaviorContext.HasThreat = false;
                _behaviorContext.Threats.Clear();
                _behaviorContext.ThreatRadius.Clear();
                _behaviorContext.ThreatPosition = Vector3.zero;
            }

            // force follow NOW, overriding WalkAway or anything else
            SetState<SheepFollowState>();
            OnRejoinedHerd();
        }

        public void SetDestinationWithHerding(Vector3 destination) => _personality.SetDestinationWithHerding(destination, this, _behaviorContext);
        public Vector3 GetTargetNearPlayer() => _personality.GetFollowTarget(this, _behaviorContext);
        public Vector3 GetGrazeTarget() => _personality.GetGrazeTarget(this, _behaviorContext);
        public Vector3 GetTargetOutsideOfHerd() => _personality.GetWalkAwayTarget(this, _behaviorContext);


        /// <summary>
        /// Updates the behavior context with current state information
        /// </summary>
        private void UpdateBehaviorContext()
        {
            if (_behaviorContext == null) return;

            _behaviorContext.PlayerPosition = _playerCenter;
            _behaviorContext.PlayerHalfExtents = _playerHalfExtents;
            _behaviorContext.DistanceToPlayer = Vector3.Distance(transform.position, _playerCenter);
            _behaviorContext.IsPlayerMoving = false; 
            _behaviorContext.HasThreat = false; 
            _behaviorContext.ThreatPosition = Vector3.zero;
            _behaviorContext.TimeSinceLastAction = Time.time;
            _behaviorContext.NeighborCount = _neighbours.Count;
            _behaviorContext.CurrentVelocity = _agent?.velocity ?? Vector3.zero;
            _behaviorContext.CurrentSpeed = _agent?.speed ?? 0f;
            _behaviorContext.IsInHerd = !FlockingUtility.IsOutSquare(transform.position, _playerCenter, _playerHalfExtents);

            // Calculate average neighbor distance
            if (_neighbours.Count > 0)
            {
                float totalDistance = 0f;
                foreach (var neighbor in _neighbours)
                {
                    if (neighbor != null)
                    {
                        totalDistance += Vector3.Distance(transform.position, neighbor.position);
                    }
                }
                _behaviorContext.AverageNeighborDistance = totalDistance / _neighbours.Count;
            }
            else
            {
                _behaviorContext.AverageNeighborDistance = float.MaxValue;
            }

            if (_threats.Count > 0)
            {
                var stale = _threats.Where(kv => Time.time - kv.Value > _threatMemory)
                    .Select(kv => kv.Key).ToList();
                foreach (var t in stale)
                {
                    _threats.Remove(t);
                    _threatRadius.Remove(t);
                }
            }

            _behaviorContext.Threats.Clear();
            _behaviorContext.ThreatRadius.Clear();
            foreach (var kv in _threats)
            {
                if (kv.Key) _behaviorContext.Threats.Add(kv.Key);
            }
            foreach (var kv in _threatRadius)
            {
                if (kv.Key) _behaviorContext.ThreatRadius[kv.Key] = kv.Value;
            }

            bool hasThreat = _behaviorContext.Threats.Count > 0;

            if (_currentState is SheepScaredState)
            {
                if (_panicLoop != null)
                {
                    StopPanicLoop();
                }

                _behaviorContext.HasThreat = false;
            }
            else
            {
                _behaviorContext.HasThreat = hasThreat;
            }

            if (_behaviorContext.HasThreat && !_hadThreatLastFrame)
            {
                StartPanicLoop();
            }
            else if (!_behaviorContext.HasThreat && _hadThreatLastFrame)
            {
                StopPanicLoop();
            }

            _hadThreatLastFrame = _behaviorContext.HasThreat;
        }

        /// <summary>
        /// Called when player performs an action (whistle, etc.)
        /// Can be called from external systems
        /// </summary>
        public void OnPlayerAction(string actionType)
        {
            _personality.OnPlayerAction(actionType, this, _behaviorContext);
        }

        /// <summary>
        /// Called when a threat is detected
        /// Can be called from external systems
        /// </summary>
        public void OnThreatDetected(Vector3 threatPosition)
        {
            _behaviorContext.HasThreat = true;
            _behaviorContext.ThreatPosition = threatPosition;
            _personality.OnThreatDetected(threatPosition, this, _behaviorContext);
        }

        /// <summary>
        /// Called when sheep gets separated from herd
        /// </summary>
        public void OnSeparatedFromHerd(bool wasLost, bool forced)
        {
            EventManager.Broadcast(new SheepLeaveHerdEvent(this, wasLost, forced));
            if (_personality != null)
            {
                _personality.OnSeparatedFromHerd(this, _behaviorContext);
            }
        }

        /// <summary>
        /// Called when sheep rejoins herd
        /// </summary>
        public void OnRejoinedHerd()
        {
            _personality.OnRejoinedHerd(this, _behaviorContext);
        }

        public void OnSheepFreeze()
        {
            SetState<SheepFreezeState>();
        }

        public void OnSheepUnfreeze()
        {
            SetState<SheepGrazeState>();
        }

        public void OnSheepDie()
        {
            _personality.OnDeath(this, _behaviorContext);
        }
        
#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            // Draw player square
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(
                _playerCenter,
                new Vector3(_playerHalfExtents.x * 2f, 0.1f, _playerHalfExtents.y * 2f)
            );

            // Draw agent destination
            if (_agent != null && _agent.isOnNavMesh)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(transform.position, _agent.destination);
                Gizmos.DrawSphere(_agent.destination, 0.2f);
            }

            if (_config != null && _config.DeathDistance > 0f && _playerCenter != Vector3.zero)

            {
                Gizmos.color = new Color(1f, 0f, 0f, 0.35f);
                Vector3 center = _playerCenter;
                if (center == Vector3.zero && Application.isPlaying == false)
                    center = transform.position;
                Gizmos.DrawWireSphere(center, _config.DeathDistance);
            }
        }
#endif

        /// <summary>
        /// Removes the sheep from the game.
        /// Broadcasts a SheepDeathEvent before destroying the GameObject.
        /// </summary>
        public void Remove(bool countTowardSanity = true)
        {
            EventManager.Broadcast(new SheepLeaveHerdEvent(this, wasLost: false, forced: true));
            EventManager.Broadcast(new SheepDeathEvent(this, countTowardSanity));
            Destroy(gameObject);
        }

        /// <summary>
        /// Check for disabling the agent
        /// </summary>
        public bool CanControlAgent()
        {
            return Agent != null
                   && Agent.enabled
                   && Agent.isOnNavMesh
                   && gameObject.activeInHierarchy;
        }

        public void MoveToPoint(Vector3 target, float stopDistance = 0.5f)
        {
            if (StatesMap == null)
                InitializeStatesMap();

            if (!StatesMap.TryGetValue(typeof(SheepMoveState), out var state))
            {
                return;
            }
            
            var moveState = state as SheepMoveState;
            if (moveState == null)
            {
                return;
            }
            
            moveState.Configure(target, stopDistance);
            SetState<SheepMoveState>();
        }

        public IState GetState() => _currentState;
    }
}

