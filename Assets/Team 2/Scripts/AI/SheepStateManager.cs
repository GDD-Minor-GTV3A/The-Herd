using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using Core.Events;
using Core.Shared.StateMachine;
using Core.AI.Sheep.Config;
using Core.AI.Sheep.Personality;
using UnityEngine.AI;

using Random = UnityEngine.Random;
using System.Xml.Serialization;

namespace Core.AI.Sheep
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class SheepStateManager : StateManager<IState>
    {
        private const float DEFAULT_FOLLOW_DISTANCE = 1.8f;
        private const float DEFAULT_MAX_LOST_DISTANCE_FROM_HERD = 10f;
        private const float DEFAULT_WALK_AWAY_FROM_HERD_TICKS = 2f;

        [Header("Data")]
        [SerializeField] private SheepConfig _config;
        [SerializeField] private SheepArchetype _archetype;
        [SerializeField] private SheepAnimationDriver _animation;

        [Header("Sounds")]
        [SerializeField] private SheepSoundDriver _sheepSoundDriver;

        [Header("Neighbours")]
        [SerializeField] private List<Transform> _neighbours = new List<Transform>();

        private readonly Dictionary<Transform, float> _threats = new();
        private readonly Dictionary<Transform, float> _threatRadius = new();
        [SerializeField] private float _threatMemory = 1.25f;
        private Coroutine _panicLoop;
        private bool _hadThreatLastFrame;

        private NavMeshAgent _agent;
        private Coroutine _tickCoroutine;
        private float _nextWalkingAwayFromHerdAt;

        private Vector3 _playerCenter;
        private Vector3 _playerHalfExtents;

        private ISheepPersonality _personality;
        private PersonalityBehaviorContext _behaviorContext;

        [SerializeField] private bool _startAsStraggler;
        [SerializeField] private float _joinGrace = 3f;
        private float _walkAwayReenableAt;

        // debugging / stuck handling
        private float _lastMovedAt;
        private const float STUCK_SPEED_EPS = 0.05f;
        private const float STUCK_TIMEOUT = 0.8f;

        public NavMeshAgent Agent => _agent;
        public SheepAnimationDriver Animation => _animation;
        public SheepConfig Config => _config;
        public SheepArchetype Archetype => _archetype;
        public ISheepPersonality Personality => _personality;
        public IReadOnlyList<Transform> Neighbours => _neighbours;
        public PersonalityBehaviorContext BehaviorContext => _behaviorContext;

        public void MarkAsStraggler() => _startAsStraggler = true;

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            if (_config != null)
                _agent.speed = _config.BaseSpeed;

            _agent.autoBraking = false;
            _agent.autoRepath = true;
            _agent.acceleration = Mathf.Max(_agent.acceleration, 8f);
            _agent.angularSpeed = Mathf.Max(_agent.angularSpeed, 1080f);

            if (_archetype?.AnimationOverrides != null)
                _animation?.ApplyOverrideController(_archetype.AnimationOverrides);

            _personality = _archetype?.CreatePersonality(this);
            _behaviorContext = new PersonalityBehaviorContext();

            InitializeStatesMap();

            Debug.Log($"[{name}] Awake: speed={_agent.speed} accel={_agent.acceleration} ang={_agent.angularSpeed}");
        }

        private void LateUpdate()
        {
            if (_animation == null || _agent == null) return;
            Vector3 v = _agent.velocity;
            v.y = 0f;
            _animation.SetSpeed(v.magnitude);
            if (v.magnitude > STUCK_SPEED_EPS)
                _lastMovedAt = Time.time;
        }

        private void OnEnable()
        {
            if (StatesMap == null || StatesMap.Count == 0)
            {
                InitializeStatesMap();
                Debug.Log($"[{name}] Re-initialized state map on enable.");
            }

            EnableBehavior();
            SetState<SheepGrazeState>();
        }

        private void OnDisable() => DisableBehavior();

        public void EnableBehavior()
        {
            EventManager.AddListener<PlayerSquareChangedEvent>(OnPlayerSquareChanged);
            EventManager.AddListener<PlayerSquareTickEvent>(OnPlayerSquareTick);

            if (_tickCoroutine == null)
            {
                float interval = _config != null ? _config.Tick : 0.15f;
                _tickCoroutine = StartCoroutine(TickCoroutine(interval));
            }
        }

        public void DisableBehavior()
        {
            EventManager.RemoveListener<PlayerSquareChangedEvent>(OnPlayerSquareChanged);
            EventManager.RemoveListener<PlayerSquareTickEvent>(OnPlayerSquareTick);

            if (_tickCoroutine != null)
            {
                StopCoroutine(_tickCoroutine);
                _tickCoroutine = null;
            }
        }

        protected override void InitializeStatesMap()
        {
            StatesMap = new Dictionary<Type, IState>
            {
                { typeof(SheepFollowState), new SheepFollowState(this) },
                { typeof(SheepGrazeState), new SheepGrazeState(this) },
                { typeof(SheepWalkAwayFromHerdState), new SheepWalkAwayFromHerdState(this) },
                { typeof(SheepFreezeState), new SheepFreezeState(this) },
                { typeof(SheepPanicState), new SheepPanicState(this) },
            };
        }

        public void SetNeighbours(List<Transform> neighbours) =>
            _neighbours = neighbours ?? new List<Transform>();

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
            if (_currentState is SheepWalkAwayFromHerdState) return;

            bool outside = FlockingUtility.IsOutSquare(transform.position, _playerCenter, _playerHalfExtents);
            Type targetState = outside ? typeof(SheepFollowState) : typeof(SheepGrazeState);
            if (_currentState.GetType() == targetState) return;

            if (outside) SetState<SheepFollowState>();
            else SetState<SheepGrazeState>();
        }

        private void OnSheepCallBackToPlayerEvent() => SetState<SheepFollowState>();

        private void TryResetSanity()
        {
            var s = GetComponent<SheepSanity>();
            if (s != null)
            {
                s.ForceReset();
                Debug.Log($"[{name}] Sanity force reset on herd rejoin");
            }
        }

        private IEnumerator TickCoroutine(float interval)
        {
            var wait = new WaitForSeconds(interval);
            while (true)
            {
                UpdateBehaviorContext();

                // check if we just came back from panic
                if (_currentState is SheepPanicState)
                {
                    if (!_behaviorContext.HasThreat && IsInHerd())
                    {
                        Debug.Log($"[{name}] Rejoined herd post-panic → SummonToHerd + reset sanity");
                        SummonToHerd(graceSeconds: _joinGrace, clearThreats: true);
                        TryResetSanity();
                    }
                }

                if (_currentState is not SheepWalkAwayFromHerdState
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
                        _sheepSoundDriver.TryPlayWalkSound(transform);

                    _sheepSoundDriver.TryPlayBleatSound(transform, _archetype);
                }

                yield return wait;
            }
        }

        public void ReportThreat(Transform enemy, Vector3 pos, float radius)
        {
            if (enemy == null) return;
            Debug.Log($"[{name}] ReportThreat enemy={enemy.name} pos={pos}");
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
            Debug.Log($"[{name}] ForgetThreat enemy={(enemy ? enemy.name : "null")} count={_threats.Count}");
            if (_threats.Count == 0)
            {
                _behaviorContext.HasThreat = false;
                _behaviorContext.ThreatPosition = Vector3.zero;
            }
        }

        private void StartPanicLoop()
        {
            if (_panicLoop != null)
            {
                Debug.Log($"[{name}] StartPanicLoop ignored, already running");
                return;
            }

            if (CanControlAgent())
            {
                Agent.autoRepath = true;
                Agent.acceleration = Mathf.Max(Agent.acceleration, (Config?.BaseSpeed ?? 2.2f) * 6f);
                Agent.angularSpeed = Mathf.Max(Agent.angularSpeed, 1080f);
                Agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
            }

            Debug.Log($"[{name}] StartPanicLoop()");
            _panicLoop = StartCoroutine(PanicSteerLoop());
        }

        private void StopPanicLoop()
        {
            if (_panicLoop == null) return;
            StopCoroutine(_panicLoop);
            _panicLoop = null;
            if (CanControlAgent()) Agent.updateRotation = true;
            Debug.Log($"[{name}] StopPanicLoop()");
        }

        private IEnumerator PanicSteerLoop()
        {
            var wait = new WaitForSeconds(0.03f);
            const float cooloff = 0.15f;
            float lastThreatSeenAt = Time.time;
            Debug.Log($"[{name}] PanicSteerLoop START");

            while (true)
            {
                if (_behaviorContext.HasThreat)
                    lastThreatSeenAt = Time.time;

                _personality.SetDestinationWithHerding(transform.position, this, _behaviorContext);

                // 🔍 stuck detection
                if (_agent != null && _agent.isOnNavMesh)
                {
                    if (Time.time - _lastMovedAt > STUCK_TIMEOUT && !_agent.pathPending)
                    {
                        Debug.LogWarning($"[{name}] STUCK detected → ResetPath + small repath");
                        _agent.ResetPath();
                        Vector3 dest = transform.position + Random.insideUnitSphere * 3f;
                        dest.y = transform.position.y;
                        SetDestinationWithHerding(dest);
                    }
                }

                if (!_behaviorContext.HasThreat && Time.time - lastThreatSeenAt > cooloff)
                {
                    Debug.Log($"[{name}] PanicSteerLoop cooloff done");
                    break;
                }

                yield return wait;
            }

            _panicLoop = null;
            Debug.Log($"[{name}] PanicSteerLoop END");
        }

        private void ScheduleNextWalkAwayFromHerd()
        {
            _nextWalkingAwayFromHerdAt = Time.time + _config?.WalkAwayFromHerdTicks ?? DEFAULT_WALK_AWAY_FROM_HERD_TICKS;
        }

        public bool IsCurrentlyOutsideHerd() =>
            FlockingUtility.IsOutSquare(transform.position, _playerCenter, _playerHalfExtents);

        public void SummonToHerd(float? graceSeconds = null, bool clearThreats = true)
        {
            _startAsStraggler = false;
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

            SetState<SheepFollowState>();
            OnRejoinedHerd();
        }

        public void SetDestinationWithHerding(Vector3 destination) =>
            _personality.SetDestinationWithHerding(destination, this, _behaviorContext);
        public Vector3 GetTargetNearPlayer() => _personality.GetFollowTarget(this, _behaviorContext);
        public Vector3 GetGrazeTarget() => _personality.GetGrazeTarget(this, _behaviorContext);
        public Vector3 GetTargetOutsideOfHerd() => _personality.GetWalkAwayTarget(this, _behaviorContext);

        private void UpdateBehaviorContext()
        {
            if (_behaviorContext == null) return;

            _behaviorContext.PlayerPosition = _playerCenter;
            _behaviorContext.PlayerHalfExtents = _playerHalfExtents;
            _behaviorContext.DistanceToPlayer = Vector3.Distance(transform.position, _playerCenter);
            _behaviorContext.IsPlayerMoving = false;
            _behaviorContext.TimeSinceLastAction = Time.time;
            _behaviorContext.NeighborCount = _neighbours.Count;
            _behaviorContext.CurrentVelocity = _agent?.velocity ?? Vector3.zero;
            _behaviorContext.CurrentSpeed = _agent?.speed ?? 0f;
            _behaviorContext.IsInHerd = !FlockingUtility.IsOutSquare(transform.position, _playerCenter, _playerHalfExtents);

            if (_neighbours.Count > 0)
            {
                float totalDistance = 0f;
                foreach (var n in _neighbours)
                    if (n != null) totalDistance += Vector3.Distance(transform.position, n.position);
                _behaviorContext.AverageNeighborDistance = totalDistance / _neighbours.Count;
            }
            else _behaviorContext.AverageNeighborDistance = float.MaxValue;

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
                if (kv.Key) _behaviorContext.Threats.Add(kv.Key);
            foreach (var kv in _threatRadius)
                if (kv.Key) _behaviorContext.ThreatRadius[kv.Key] = kv.Value;

            bool hadBefore = _behaviorContext.HasThreat;
            _behaviorContext.HasThreat = _behaviorContext.Threats.Count > 0;

            if (_behaviorContext.HasThreat && !_hadThreatLastFrame)
            {
                Debug.Log($"[{name}] THREAT APPEARED → StartPanicLoop");
                StartPanicLoop();
            }
            else if (!_behaviorContext.HasThreat && _hadThreatLastFrame)
            {
                Debug.Log($"[{name}] THREAT CLEARED → StopPanicLoop");
                StopPanicLoop();
            }

            _hadThreatLastFrame = _behaviorContext.HasThreat;
        }

        public void OnPlayerAction(string actionType) =>
            _personality.OnPlayerAction(actionType, this, _behaviorContext);

        public void OnThreatDetected(Vector3 threatPosition)
        {
            _behaviorContext.HasThreat = true;
            _behaviorContext.ThreatPosition = threatPosition;
            _personality.OnThreatDetected(threatPosition, this, _behaviorContext);
        }

        public void OnSeparatedFromHerd() => _personality.OnSeparatedFromHerd(this, _behaviorContext);
        public void OnRejoinedHerd() => _personality.OnRejoinedHerd(this, _behaviorContext);
        public void OnSheepFreeze() => SetState<SheepFreezeState>();
        public void OnSheepUnfreeze() => SetState<SheepGrazeState>();

        public bool CanControlAgent() =>
            Agent != null && Agent.enabled && Agent.isOnNavMesh && gameObject.activeInHierarchy;

        public IState GetState() => _currentState;

        public bool IsInHerd() =>
            !FlockingUtility.IsOutSquare(transform.position, _playerCenter, _playerHalfExtents);

        private bool _lockedExternally;

        public void LockStateFromExternal()
        {
            _lockedExternally = true;
        }

        public void UnlockStateFromExternal()
        {
            _lockedExternally = false;
        }

        public bool IsLockedExternally => _lockedExternally;
    }
    
}
