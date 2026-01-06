using UnityEngine;
using Core.Shared.StateMachine;
using UnityEngine.AI;
using System.Collections.Generic;

using Core.AI.Sheep.Event;
using Core.Events;

using NUnit.Framework;

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
        private bool _hasTarget;
        private float _nextGrazeAt;
        private const float REACH_THRESHOLD = 0.35f;

#if UNITY_EDITOR
        private Vector3 _lastLoggedTarget;
#endif
        public SheepGrazeState(SheepStateManager context)
        {
            _stateManager = context;
        }

        public void OnStart()
        {
            if (_stateManager == null) return;
            if (_stateManager.Agent != null &&
                _stateManager.Animation != null &&
                _stateManager.CanControlAgent())
            {
                _stateManager.Agent.isStopped = false;
            }
            
            PickNewGrazeTarget();
        }

        public void OnUpdate()
        {
            if (_stateManager == null) return;

            var agent = _stateManager.Agent;
            if (agent == null) return;

            if (!_hasTarget)
            {
                PickNewGrazeTarget();
                return;
            }

            bool arrived = HasArrived();

            if (!arrived)
            {
                if (_stateManager.CanControlAgent())
                {
                    agent.isStopped = false;
                    _stateManager.SetDestinationWithHerding(_currentTarget);
                }
                return;
            }

            if (_stateManager.CanControlAgent())
            {
                agent.isStopped = true;
                agent.velocity = Vector3.zero;
            }

            if (Time.time >= _nextGrazeAt)
            {
                PickNewGrazeTarget();
            }
        }

        public void OnStop()
        {
            if (_stateManager.CanControlAgent())
            {
                _stateManager.Agent.isStopped = false;
            }
        }

        private void PickNewGrazeTarget()
        {
            _currentTarget = _stateManager.GetGrazeTarget();
            _hasTarget = true;
            
#if UNITY_EDITOR
            if ((_currentTarget - _lastLoggedTarget).sqrMagnitude > 0.01f)
            {
                Debug.Log($"[{_stateManager.name}] NEW GRAZE TARGET at frame {Time.frameCount}: {_currentTarget}");
                _lastLoggedTarget = _currentTarget;
            }
#endif

            if (_stateManager.CanControlAgent())
            {
                _stateManager.Agent.isStopped = false;
                _stateManager.SetDestinationWithHerding(_currentTarget);
            }
            ScheduleNextGraze();
        }

        private void ScheduleNextGraze()
        {
            float min = Mathf.Max(0.3f, _stateManager.Archetype?.GrazeIntervalMin ?? 3f);
            float max = Mathf.Max(min, _stateManager.Archetype?.GrazeIntervalMax ?? 5f);
            float baseInterval = Random.Range(min, max);

            _nextGrazeAt = Time.time + baseInterval;
        }

        private bool HasArrived()
        {
            var agent = _stateManager.Agent;
            if (agent == null) return false;
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

    public sealed class SheepMoveState : IState
    {
        private readonly SheepStateManager _stateManager;

        private Vector3 _target;
        private float _stopDistance = 0.5f;
        private bool _hasTarget;

        public SheepMoveState(SheepStateManager context)
        {
            _stateManager = context;
        }

        public void Configure(Vector3 target, float stopDistance)
        {
            _target = target;
            _stopDistance = Mathf.Max(0.1f, stopDistance);
            _hasTarget = true;
        }

        public void OnStart()
        {
            if (!_stateManager || !_stateManager.CanControlAgent()) return;

            var agent = _stateManager.Agent;
            agent.isStopped = false;
            agent.ResetPath();
        }

        public void OnUpdate()
        {
            if (!_hasTarget) return;
            if (!_stateManager || !_stateManager.CanControlAgent()) return;
            
            var agent =  _stateManager.Agent;
            _stateManager.SetDestinationWithHerding(_target);
            if (agent.pathPending) return;
            
            Vector3 toTarget = _target - _stateManager.transform.position;
            toTarget.y = 0f;

            if (toTarget.sqrMagnitude <= _stopDistance * _stopDistance)
            {
                _stateManager.OnSheepFreeze();
            }
        }

        public void OnStop()
        {
            if (_stateManager && _stateManager.CanControlAgent())
            {
                _stateManager.Agent.isStopped = false;
            }
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
        private const float RUN_DISTANCE = 85f;
        private const float NAVMESH_SEARCH_RADIUS = 25f;

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
            
            Vector3 pos = _stateManager.transform.position;
            Vector3 fromSource = pos - _scareHandler.LastScareSource;
            fromSource.y = 0f;

            if (fromSource.sqrMagnitude < 0.0001f)
            {
                fromSource = _stateManager.transform.forward;
                fromSource.y = 0f;
            }

            if (fromSource.sqrMagnitude < 0.0001f)
            {
                Vector2 r = Random.insideUnitCircle.normalized;
                fromSource = new Vector3(r.x, 0f, r.y);
            }
            
            fromSource.Normalize();
            
            Vector3 safeSpot = FindFleeDestination();
            _stateManager.Agent.isStopped = false;
            float baseSpeed = _stateManager.Config?.BaseSpeed ?? 2.2f;
            _stateManager.Agent.speed = baseSpeed * 2f;
            _stateManager.Agent.SetDestination(safeSpot);
            _hasDestination = true;
            float distance = Vector3.Distance(pos, safeSpot);
            float runSpeed = Mathf.Max(_stateManager.Agent.speed, 0.1f);
            float travelTime = distance / runSpeed;
            _endTime = Time.time + travelTime * 1.5f;
        }

        public void OnUpdate()
        {
            if (!_stateManager || !_stateManager.CanControlAgent()) return;
            if (!_hasDestination) return;
            
            var agent = _stateManager.Agent;
            if (!agent.pathPending && agent.remainingDistance <= STOP_THRESHOLD)
            {
                EndPanic();
                return;
            }

            if (Time.time >= _endTime)
            {
                EndPanic();
            }
        }

        public void OnStop()
        {
            if (_stateManager.CanControlAgent())
            {
                float baseSpeed = _stateManager.Config?.BaseSpeed ?? 2.2f;
                _stateManager.Agent.speed = baseSpeed;
                _stateManager.Agent.isStopped = false;
            }
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
        
        private Vector3 FindFleeDestination()
        {
            Vector3 pos     = _stateManager.transform.position;
            Vector3 danger  = _scareHandler.LastScareSource;
            
            Vector3 dir = (pos - danger);
            dir.y = 0f;
            if (dir.sqrMagnitude < 0.001f)
            {
                Vector2 rnd = Random.insideUnitCircle.normalized;
                dir = new Vector3(rnd.x, 0f, rnd.y);
            }
            dir.Normalize();

            float runDist        = RUN_DISTANCE;
            float minAcceptable  = 50f;
            float searchRadius   = 50f;

            for (int i = 0; i < 12; i++)
            {
                Vector3 testDir   = Quaternion.Euler(0, i * 30f, 0) * dir;
                Vector3 rawTarget = pos + testDir * runDist;
                
                if (!FlockingUtility.IsOutSquare(rawTarget, _stateManager.PlayerSquareCenter, _stateManager.PlayerSquareHalfExtents))
                {
                    Vector3 fromPlayer = rawTarget - _stateManager.PlayerSquareCenter;
                    fromPlayer.y = 0f;
                    float overshoot = Mathf.Max(_stateManager.PlayerSquareHalfExtents.x, _stateManager.PlayerSquareHalfExtents.z) + minAcceptable;
                    rawTarget = _stateManager.PlayerSquareCenter + fromPlayer.normalized * overshoot;
                }
                
                if (NavMesh.SamplePosition(rawTarget, out NavMeshHit hit, searchRadius, NavMesh.AllAreas))
                {
                    if (Vector3.Distance(pos, hit.position) > minAcceptable)
                        return hit.position;
                }
            }
            
            return pos + dir * minAcceptable;
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