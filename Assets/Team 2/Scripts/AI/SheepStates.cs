using Core.Shared.StateMachine;

using UnityEngine;
using UnityEngine.AI;

namespace Core.AI.Sheep
{
    /// <summary>
    /// Following player if outside of the square
    /// </summary>
    public sealed class SheepFollowState : IState
    {
        private readonly SheepStateManager _stateManager;

        public SheepFollowState(SheepStateManager context)
        {
            _stateManager = context;
        }

        public void OnStart()
        {
            //for animations, sounds and so on
            if (_stateManager.CanControlAgent())
            {
                _stateManager.Agent.isStopped = false;
            }
            _stateManager.Agent.isStopped = false;
            _stateManager.Animation?.SetState((int)SheepAnimState.Walk);
        }

        public void OnUpdate()
        {
            if (_stateManager == null) { return; }
            ;
            Vector3 target = _stateManager.GetTargetNearPlayer();
            _stateManager.SetDestinationWithHerding(target);
        }

        public void OnStop() { }
    }


    /// <summary>
    /// Grazing in the square
    /// </summary>
    public sealed class SheepGrazeState : IState
    {
        private readonly SheepStateManager _stateManager;
        private Vector3 _currentTarget;
        private float _nextGrazeAt;
        private const float REACH_THRESHOLD = 0.35f;

        public SheepGrazeState(SheepStateManager context)
        {
            _stateManager = context;
        }

        public void OnStart()
        {
            ScheduleNextGraze();
            if (_stateManager.Agent != null && _stateManager?.Animation != null && _stateManager.CanControlAgent())
            {
                _stateManager.Agent.isStopped = false;
                _stateManager.Animation.SetState((int)SheepAnimState.Idle);
            }
        }

        public void OnUpdate()
        {
            if (_stateManager == null) { return; }

            if (Time.time < _nextGrazeAt)
            {
                if (HasArrived() && _stateManager.CanControlAgent())
                {
                    _stateManager.Agent.isStopped = true;
                }
                return;
            }

            _stateManager.Agent.isStopped = false;
            _currentTarget = _stateManager.GetGrazeTarget();
            _stateManager.SetDestinationWithHerding(_currentTarget);
            _stateManager.Animation?.SetState((int)SheepAnimState.Walk);
            ScheduleNextGraze();
        }

        public void OnStop()
        {
            var agent = _stateManager.Agent;
            if (_stateManager.CanControlAgent())
            {
                _stateManager.Agent.isStopped = false;
            }
        }

        private void ScheduleNextGraze()
        {
            float min = Mathf.Max(0.1f, _stateManager.Archetype?.GrazeIntervalMin ?? 3f);
            float max = Mathf.Max(min, _stateManager.Archetype?.GrazeIntervalMax ?? 5f);
            float baseInterval = Random.Range(min, max);

            _nextGrazeAt = Time.time + baseInterval;
        }

        private bool HasArrived()
        {
            var agent = _stateManager.Agent;
            if (agent.pathPending) return false;
            return agent.remainingDistance <= REACH_THRESHOLD;
        }
    }

    /// <summary>
    /// Move outside the square (herd)
    /// </summary>
    public sealed class SheepWalkAwayFromHerdState : IState
    {
        private readonly SheepStateManager _stateManager;
        private Vector3 _currentTarget;


        public SheepWalkAwayFromHerdState(SheepStateManager context)
        {
            _stateManager = context;
        }

        public void OnStart()
        {
            if (_stateManager == null) return;

            _currentTarget = _stateManager.GetTargetOutsideOfHerd();

            if (_stateManager?.Agent != null && _stateManager?.Animation != null && _stateManager.CanControlAgent())
            {
                _stateManager.Agent.isStopped = false;
                _stateManager.Animation.SetState((int)SheepAnimState.Walk);
            }
        }

        public void OnUpdate()
        {
            if (_stateManager == null || _stateManager.Agent == null) return;
            if (_stateManager.CanControlAgent())
            {
                _stateManager.Agent.isStopped = false;
            }
            _stateManager.Agent.SetDestination(_currentTarget);

        }

        public void OnStop()
        {
            if (_stateManager?.Agent != null && _stateManager.CanControlAgent())
            {
                _stateManager.Agent.isStopped = true;
            }
        }
    }
}