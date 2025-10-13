using UnityEngine;
using UnityEngine.AI;

namespace AI.Shaman
{
    /// <summary>
    /// shaman walks around in a defined area.
    /// </summary>
    public class ShamanWalkAround : ShamanState
    {
        private readonly NavMeshAgent _agent;
        private readonly Transform _transform;
        private Transform _playerTransform;
        private Vector3 _dest;

        private const float RANGE = 6f;
        private const int DEFAULT_LAYER = 1;


        public ShamanWalkAround(ShamanStateManager stateMachine) : base(stateMachine)
        {
            _agent = _manager.Shaman.GetComponent<NavMeshAgent>();
            _transform = _manager.Shaman.transform;

            // find player
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                _playerTransform = playerObj.transform;
        }


        public override void OnStart()
        {
            if (_agent != null && _agent.isOnNavMesh)
            {
                _agent.isStopped = false;
            }

            float z = Random.Range(-RANGE, RANGE);
            float x = Random.Range(-RANGE, RANGE);
            _dest = new Vector3(_transform.position.x + x, _transform.position.y, _transform.position.z + z);

            // if (Physics.Raycast(_dest, Vector3.down, out RaycastHit hit, _defaultLayer))
            // {
            //     _dest = hit.point;
            // }
        }

        public override void OnStop()
        {
        }

        public override void OnUpdate()
        {
            _agent.SetDestination(_dest);

            if (!_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance)
            {
                _manager.SetState<ShamanIdle>();
            }
        }
    }
}

