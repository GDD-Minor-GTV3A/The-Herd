using UnityEngine;
using Core.Shared.StateMachine;
using UnityEngine.AI;
using System.Collections.Generic;

using Core.AI.Sheep.Event;
using Core.Events;

namespace Core.AI.Sheep
{
    /// <summary>
    /// Following player if outside the square
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
            //_stateManager.Animation?.SetState((int)SheepAnimState.Walk);
        }

        public void OnUpdate()
        {
            if (_stateManager == null) { return; };
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
            _nextGrazeAt = Time.time + Random.Range(0f, 0.8f);
            if(_stateManager.Agent !=  null && _stateManager?.Animation != null && _stateManager.CanControlAgent())
            {
                _stateManager.Agent.isStopped = false;
                //_stateManager.Animation.SetState((int)SheepAnimState.Idle);
            }
        }

        public void OnUpdate()
        {
            if (_stateManager == null) { return; }

            if(Time.time < _nextGrazeAt)
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
            //_stateManager.Animation?.SetState((int)SheepAnimState.Walk);
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
            if(agent.pathPending) return false;
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
            if (!_stateManager) return;
            _stateManager.OnSeparatedFromHerd(wasLost: true, forced: false);

            _currentTarget = _stateManager.GetTargetOutsideOfHerd();

            if (_stateManager?.Agent && _stateManager?.Animation && _stateManager.CanControlAgent())
            {
                _stateManager.Agent.isStopped = false;
                //_stateManager.Animation.SetState((int)SheepAnimState.Walk);
                if (_stateManager.Archetype != null && _stateManager.Archetype.DeathSound != null)
                {
                    float pitch = Random.Range(0.9f, 1.05f);
                    _stateManager.SoundDriver.PlayMiscSound(
                        _stateManager.Archetype.DeathSound,
                        1.0f,
                        pitch);
                }
            }
        }

        public void OnUpdate()
        {
            if (!_stateManager || !_stateManager.Agent) return;
            if (_stateManager.CanControlAgent())
            {
                _stateManager.Agent.isStopped = false;
            }
            _stateManager.Agent.SetDestination(_currentTarget);

        }

        public void OnStop()
        {
            if(_stateManager?.Agent && _stateManager.CanControlAgent())
            {
                _stateManager.Agent.isStopped = true;
            }
        }
    }

    /// <summary>
    /// Freeze the sheep in place, disabling all behavior
    /// </summary>
    public class SheepFreezeState : IState
    {
        private readonly SheepStateManager _stateManager;

        public SheepFreezeState(SheepStateManager context)
        {
            _stateManager = context;
        }

        public void OnStart()
        {
            if (_stateManager.CanControlAgent())
            {
                _stateManager.Agent.ResetPath();
                _stateManager.Agent.isStopped = true;
            }

            //_stateManager.Animation?.SetState((int)SheepAnimState.Idle);
            _stateManager.DisableBehavior();
        }

        public void OnUpdate() {} // Sheep remains frozen, no updates needed

        public void OnStop()
        {
            if (_stateManager.CanControlAgent())
            {
                _stateManager.Agent.isStopped = false;
            }

            _stateManager.EnableBehavior();
        }
    }

    public sealed class SheepPettingState : IState
    {
        private readonly SheepStateManager _stateManager;
        private bool _isPettingComplete = false;

        public SheepPettingState(SheepStateManager context)
        {
            _stateManager = context;
        }

        public void OnStart()
        {
            _isPettingComplete = false;

            if (_stateManager.CanControlAgent())
            {
                _stateManager.Agent.isStopped = true;
                _stateManager.Agent.velocity = Vector3.zero;
            }
            
            //Animation hook

            if (_stateManager.Archetype != null && _stateManager.Archetype.PettingSound != null)
            {
                float pitch = Random.Range(0.9f, 1.05f);
                _stateManager.SoundDriver.PlayMiscSound(
                    _stateManager.Archetype.PettingSound,
                    1.0f,
                    pitch);
            }
            
            EventManager.Broadcast(new ShowFlashbackEvent(
                _stateManager.FlashbackImage,
                OnFlashbackClosed));
        }

        public void OnUpdate()
        {
            if (_isPettingComplete)
            {
                _stateManager.SetState<SheepGrazeState>();
            }
        }

        public void OnStop()
        {
            if (_stateManager.CanControlAgent())
            {
                _stateManager.Agent.isStopped = false;
            }
        }

        private void OnFlashbackClosed()
        {
            _isPettingComplete = true;
        }
    }

    public sealed class SheepScaredState : IState
    {
        private readonly SheepStateManager _stateManager;
        private readonly SheepScareHandler _scareHandler;

        private const float STOP_THRESHOLD = 0.25f;
        private const float PANIC_DURATION = 3.0f;

        private bool _hasDestination;
        private float _endTime;

        public SheepScaredState(SheepStateManager context)
        {
            _stateManager = context;
            _scareHandler = context.GetComponent<SheepScareHandler>();
        }

        public void OnStart()
        {
            if (!_stateManager || !_stateManager.CanControlAgent()) return;

            Vector3 fromSource = (_stateManager.transform.position - _scareHandler.LastScareSource);
            fromSource.y = 0f;
            if (fromSource.sqrMagnitude < 0.0001f)
            {
                fromSource = _stateManager.transform.forward;
            }
            
            fromSource.Normalize();
            Vector3 safeSpot = _stateManager.transform.position + fromSource * 5f;

            _stateManager.Agent.isStopped = false;
            _stateManager.Agent.SetDestination(safeSpot);
            _hasDestination = true;
            _endTime = Time.time + PANIC_DURATION;
        }

        public void OnUpdate()
        {
            if (!_stateManager || !_stateManager.CanControlAgent()) return;
            if (!_hasDestination) return;

            if (!_stateManager.Agent.pathPending && _stateManager.Agent.remainingDistance <= STOP_THRESHOLD)
            {
                OnReachedSafeSpot();
            }

            if (Time.time >= _endTime)
            {
                //EndPanic();
            }
        }

        public void OnStop()
        {
            if (_stateManager.CanControlAgent())
                _stateManager.Agent.isStopped = false;
        }
        
        private void OnReachedSafeSpot()
        {
            _stateManager.SummonToHerd(graceSeconds: 2f, clearThreats: true);
        }

        private void EndPanic()
        {
            bool outsideHerd = FlockingUtility.IsOutSquare(
                _stateManager.transform.position,
                _stateManager.PlayerSquareCenter,
                _stateManager.PlayerSquareHalfExtents);
            
            if (outsideHerd)
                _stateManager.SetState<SheepFollowState>();
            else
            {
                _stateManager.SetState<SheepGrazeState>();
            }
        }

        /*private bool CalculateFleeDestination(out Vector3 result)
        {
            var scareHandler = _stateManager.GetComponent<SheepScareHandler>();
            Vector3 dangerPos = scareHandler != null ? scareHandler.LastScareSource : _stateManager.transform.position;
            Vector3 directionAway = (_stateManager.transform.position - dangerPos).normalized;

            if (directionAway.sqrMagnitude < 0.01f)
            {
                Vector2 rnd = Random.insideUnitCircle.normalized;
                directionAway = new Vector3(rnd.x, 0, rnd.y);
            }

            for (int i = 0; i < NAVMESH_ATTEMPTS; i++)
            {
                Vector3 currentDir = Quaternion.Euler(0, i * 15f, 0) * directionAway;
                Vector3 targetPos = _stateManager.transform.position + (currentDir * RUN_DISTANCE);

                if (NavMesh.SamplePosition(targetPos, out NavMeshHit hit, 5f, NavMesh.AllAreas))
                {
                    result = hit.position;
                    return true;
                }
            }
            
            result = Vector3.zero;
            return false;
        }*/
    }
    
    public class SheepDieState : IState
    {
        private readonly SheepStateManager _stateManager;

        public SheepDieState(SheepStateManager context)
        {
            _stateManager = context;
        }
        // ReSharper disable Unity.PerformanceAnalysis
        public void OnStart()
        {
            _stateManager.DisableBehavior();
            if (_stateManager.Archetype != null && _stateManager.Archetype.DeathSound != null)
            {
                float pitch = Random.Range(0.9f, 1.05f);
                _stateManager.SoundDriver.PlayMiscSound(
                    _stateManager.Archetype.DeathSound,
                    1.0f,
                    pitch);
            }
            //possible ragdoll setup in here in the next sprint
        }

        public void OnUpdate() { } //no logic needed for now in here
        
        public void OnStop() { } //no logic needed for now in here
    }
}