using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using Core.Events;
using Core.Shared.StateMachine;
using Core.AI.Sheep.Config;
using UnityEngine.AI;

using Random = UnityEngine.Random;

namespace Core.AI.Sheep
{
    /// <summary>
    /// Sheep state manager
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent))]
    public class SheepStateManager : CharacterStateManager<IState>
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
            var targetState = outside ? typeof(SheepFollowState) : typeof(SheepGrazeState);

            if(_currentState.GetType() == targetState) return;

            SetState(targetState);

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
                if (_currentState is not SheepWalkAwayFromHerdState && Time.time >= _nextWalkingAwayFromHerdAt)
                {
                    if (Random.value <= _archetype.GettingLostChance) SetState<SheepWalkAwayFromHerdState>();
                    ScheduleNextWalkAwayFromHerd();
                }
                _currentState?.OnUpdate();
                yield return wait;
            }

        }

        private void ScheduleNextWalkAwayFromHerd()
        {
            _nextWalkingAwayFromHerdAt = Time.time + _config?.WalkAwayFromHerdTicks ?? DEFAULT_WALK_AWAY_FROM_HERD_TICKS;
        }


        /// <summary>
        /// Stops last state and starts new state.
        /// </summary>
        /// <param name="type">State to change to</param>
        public virtual void SetState(Type type)
        {
            _currentState?.OnStop();

            _currentState = StatesMap[type];
            _currentState.OnStart();
        }


        #region ======== Helpers for states ========

        /// <summary>
        /// Set destination with herding nudge
        /// </summary>
        public void SetDestinationWithHerding(Vector3 destination)
        {
            Vector3 desired = destination - transform.position;
            desired.y = 0f;

            float sepDist = _config?.SeparationDistance ?? 0.8f;
            float sepW = _config?.SeparationWeight ?? 1.2f;
            float alignW = _config?.AlignmentWeight ?? 0.6f;
            float clamp = _config?.SteerClamp ?? 2.5f;

            Vector3 steer = FlockingUtility.Steering(
                transform,
                _neighbours,
                sepDist,
                sepW,
                alignW,
                clamp
            );

            Vector3 final = transform.position + desired + steer;
            _agent?.SetDestination( final );
        }

        /// <summary>
        /// Makes a destination near the player
        /// </summary>
        public Vector3 GetTargetNearPlayer()
        {
            float want = Mathf.Min(0.5f, _archetype?.FollowDistance ?? DEFAULT_FOLLOW_DISTANCE);
            Vector3 dir = (_playerCenter - transform.position);
            dir.y = 0f;
            dir = dir.sqrMagnitude > 0.001f ? dir.normalized : Vector3.forward;

            Vector3 target = _playerCenter - dir * want;

            //Reduce stacking
            target += Quaternion.Euler(0f, Random.Range(-35f, 35f), 0f) * (Vector3.right * (want * 0.5f));
            return target;
        }

        /// <summary>
        /// Pick a point to go to when grazing
        /// </summary>
        public Vector3 GetGrazeTarget()
        {
            float step = Mathf.Max(0.2f, _archetype?.IdleWanderRadius ?? 1.0f);
            Vector2 rand = Random.insideUnitCircle * step;
            return transform.position + new Vector3(rand.x, 0f, rand.y);
        }

        /// <summary>
        /// Picks a point to go to when leaving the herd.
        /// </summary>
        /// <returns></returns>
        public Vector3 GetTargetOutsideOfHerd()
        {
            float maxLostDistanceFromHerd = _config?.MaxLostDistanceFromHerd ?? DEFAULT_MAX_LOST_DISTANCE_FROM_HERD;
            Vector2 rand = Random.insideUnitCircle * maxLostDistanceFromHerd;
            Vector3 playerHalf = new (rand.x < 0 ? _playerHalfExtents.x * -1 : _playerHalfExtents.x, 0f, rand.y < 0 ? _playerHalfExtents.z * -1 : _playerHalfExtents.z);
            return transform.position  + playerHalf +  new Vector3(rand.x, 0f, rand.y);
        }

        #endregion



    }

}
