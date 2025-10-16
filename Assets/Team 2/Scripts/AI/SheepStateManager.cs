using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using Core.Events;
using Core.Shared.StateMachine;
using Core.AI.Sheep.Config;
using Core.AI.Sheep.Personality;
using UnityEngine.AI;

using Random = UnityEngine.Random;

namespace Core.AI.Sheep
{
    /// <summary>
    /// Sheep state manager
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent))]
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

        [Header("Neighbours")]
        [SerializeField]
        [Tooltip("All herd members")]
        private List<Transform> _neighbours = new List<Transform>();


        //Private
        private NavMeshAgent _agent;
        private Coroutine _tickCoroutine;
        private float _nextWalkingAwayFromHerdAt;

        private Vector3 _playerCenter;
        private Vector3 _playerHalfExtents;

        // Personality system
        private ISheepPersonality _personality;
        private PersonalityBehaviorContext _behaviorContext;


        /// <summary>
        /// Exposed NavMeshAgent for state machine
        /// </summary>
        public NavMeshAgent Agent => _agent;

        public SheepAnimationDriver Animation => _animation;

        /// <summary>
        /// Exposed config and archetype
        /// </summary>
        public SheepConfig Config => _config;
        public SheepArchetype Archetype => _archetype;
        public ISheepPersonality Personality => _personality;

        /// <summary>
        /// Read-only list of neighbouring sheep
        /// </summary>
        public IReadOnlyList<Transform> Neighbours => _neighbours;
        
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

            InitializeStatesMap();
        }

        private void LateUpdate()
        {
            if(_animation == null || _agent == null) return;
            Vector3 v = _agent.velocity;
            v.y = 0f;
            _animation.SetSpeed(v.magnitude);
        }

        private void OnEnable()
        {
            EventManager.AddListener<PlayerSquareChangedEvent>(OnPlayerSquareChanged);
            EventManager.AddListener<PlayerSquareTickEvent>(OnPlayerSquareTick);
            
            SetState<SheepGrazeState>();

            float interval = _config != null ? _config.Tick : 0.15f;
            _tickCoroutine = StartCoroutine(TickCoroutine(interval));
        }

        private void OnDisable()
        {
            EventManager.RemoveListener<PlayerSquareChangedEvent>(OnPlayerSquareChanged);
            EventManager.RemoveListener<PlayerSquareTickEvent>(OnPlayerSquareTick);

            if(_tickCoroutine != null)
            {
                StopCoroutine(_tickCoroutine);
                _tickCoroutine = null;
            }

            _currentState?.OnStop();
        }

        protected override void InitializeStatesMap()
        {
            StatesMap = new Dictionary<Type, IState>
            {
                {typeof(SheepFollowState), new SheepFollowState(this)},
                {typeof(SheepGrazeState), new SheepGrazeState(this)},
                {typeof(SheepWalkAwayFromHerdState), new SheepWalkAwayFromHerdState(this)},
            };
        }

        /// <summary>
        /// Fill list with neighbours
        /// </summary>
        public void SetNeighbours(List<Transform> neighbours)
        {
            _neighbours = neighbours ?? new List<Transform>();
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

            if (_currentState is SheepWalkAwayFromHerdState) return;

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
                // Update behavior context for personality
                UpdateBehaviorContext();

                if (_currentState is not SheepWalkAwayFromHerdState && Time.time >= _nextWalkingAwayFromHerdAt)
                {
                    if (Random.value <= _archetype.GettingLostChance) SetState<SheepWalkAwayFromHerdState>();
                    ScheduleNextWalkAwayFromHerd();
                }

                yield return wait;
            }
        }

        private void ScheduleNextWalkAwayFromHerd()
        {
            _nextWalkingAwayFromHerdAt = Time.time + _config?.WalkAwayFromHerdTicks ?? DEFAULT_WALK_AWAY_FROM_HERD_TICKS;
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
        public void OnSeparatedFromHerd()
        {
            _personality.OnSeparatedFromHerd(this, _behaviorContext);
        }

        /// <summary>
        /// Called when sheep rejoins herd
        /// </summary>
        public void OnRejoinedHerd()
        {
            _personality.OnRejoinedHerd(this, _behaviorContext);
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
    }
}

