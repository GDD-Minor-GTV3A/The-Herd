using UnityEngine;
using UnityEngine.AI;

namespace AI.Shaman
{
    /// <summary>
    /// shaman is standing idle
    /// </summary>
    public class ShamanIdle : ShamanState
    {
        private readonly NavMeshAgent _agent;
        private float _idleTimer;
        private float _idleDuration;

        private const float MIN_IDLE_TIME = 12f;
        private const float MAX_IDLE_TIME = 24f;


        public ShamanIdle(ShamanStateManager stateMachine) : base(stateMachine)
        {
            _agent = _manager.Shaman.GetComponent<NavMeshAgent>();
        }

        public override void OnStart()
        {
            if (_agent != null && _agent.isOnNavMesh)
            {
                _agent.isStopped = true;
            }

            _idleTimer = 0f;
            _idleDuration = Random.Range(MIN_IDLE_TIME, MAX_IDLE_TIME);
        }

        public override void OnStop()
        {
        }

        public override void OnUpdate()
        {
            _idleTimer += Time.deltaTime;

            if (_idleTimer >= _idleDuration)
            {
                _manager.SetState<ShamanWalkAround>();
            }
        }
    }
}

