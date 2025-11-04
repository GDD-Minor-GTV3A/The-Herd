using UnityEngine;
using Core.Shared.StateMachine;
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
            if (_stateManager.CanControlAgent())
            {
                _stateManager.Agent.isStopped = false;
            }

            _stateManager.Animation?.SetState((int)SheepAnimState.Walk);
        }

        public void OnUpdate()
        {
            if (_stateManager == null) return;

            Vector3 target = _stateManager.GetTargetNearPlayer();
            _stateManager.SetDestinationWithHerding(target);
        }

        public void OnStop() { }
    }

    /// <summary>
    /// Grazing inside the herd square.
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

            if (_stateManager.Agent != null && _stateManager.Animation != null && _stateManager.CanControlAgent())
            {
                _stateManager.Agent.isStopped = false;
                _stateManager.Animation.SetState((int)SheepAnimState.Idle);
            }
        }

        public void OnUpdate()
        {
            if (_stateManager == null) return;

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
    /// Walks away from herd temporarily.
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

            if (_stateManager.Agent != null && _stateManager.Animation != null && _stateManager.CanControlAgent())
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
            if (_stateManager.Agent != null && _stateManager.CanControlAgent())
            {
                _stateManager.Agent.isStopped = true;
            }
        }
    }

    /// <summary>
    /// Freeze the sheep in place, disabling all behavior.
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

            _stateManager.Animation?.SetState((int)SheepAnimState.Idle);
            _stateManager.DisableBehavior();
        }

        public void OnUpdate() { }

        public void OnStop()
        {
            if (_stateManager.CanControlAgent())
            {
                _stateManager.Agent.isStopped = false;
            }

            _stateManager.EnableBehavior();
        }
    }

    /// <summary>
    /// PANIC STATE – sheep flees away from the threat, then calms down and walks back to herd.
    /// </summary>
    public sealed class SheepPanicState : IState
    {
        private readonly SheepStateManager _stateManager;
        private NavMeshAgent _agent;

        private Vector3 _fleeTarget;
        private bool _isReturning;
        private bool _hasFleeTarget;
        private float _panicStartedAt;

        // Tunable settings
        private const float FleeDistance = 40f;
        private const float ReturnDistance = 5f;
        private const float PanicSpeedMultiplier = 2.4f;
        private const float ArriveThreshold = 1.2f;

        public SheepPanicState(SheepStateManager context)
        {
            _stateManager = context;
        }

        public void OnStart()
        {
            if (!_stateManager.CanControlAgent()) return;
            _agent = _stateManager.Agent;

            _stateManager.DisableBehavior();
            _stateManager.LockStateFromExternal();

            _agent.isStopped = false;
            _agent.autoBraking = false;
            _agent.speed = (_stateManager.Config?.BaseSpeed ?? 2f) * PanicSpeedMultiplier;
            _agent.acceleration = 12f;

            _stateManager.Animation?.SetState((int)SheepAnimState.Run);

            // Determine flee direction
            Vector3 herdCenter = _stateManager.BehaviorContext.PlayerPosition;
            Vector3 away = (_stateManager.transform.position - herdCenter).normalized;
            if (away.sqrMagnitude < 0.1f)
                away = Random.insideUnitSphere;
            away.y = 0f;

            Vector3 candidate = _stateManager.transform.position + away * FleeDistance;

            // ✅ Ensure the panic target is valid and reachable
            NavMeshHit hit;
            if (NavMesh.SamplePosition(candidate, out hit, 15f, NavMesh.AllAreas))
            {
                _fleeTarget = hit.position;

                if (NavMesh.CalculatePath(_stateManager.transform.position, _fleeTarget, NavMesh.AllAreas, new NavMeshPath()) &&
                    _agent.isOnNavMesh)
                {
                    _hasFleeTarget = true;
                    _agent.SetDestination(_fleeTarget);
                    Debug.Log($"[{_stateManager.name}] PANIC fleeing to {_fleeTarget}");
                }
                else
                {
                    Debug.LogWarning($"[{_stateManager.name}] Panic path invalid — picking random nearby target");
                    Vector3 fallback = _stateManager.transform.position + Random.insideUnitSphere * 4f;
                    fallback.y = _stateManager.transform.position.y;
                    _agent.SetDestination(fallback);
                }
            }
            else
            {
                Debug.LogWarning($"[{_stateManager.name}] No valid flee target on NavMesh, staying idle");
                _hasFleeTarget = false;
            }
        }


        public void OnUpdate()
        {
            if (!_stateManager.CanControlAgent() || !_hasFleeTarget)
            {
                Debug.LogWarning($"[{_stateManager.name}] Panic update skipped (no target or invalid agent)");
                return;
            }

            float elapsed = Time.time - _panicStartedAt;

            // Debug NavMesh state each frame
            Debug.Log($"[{_stateManager.name}] PanicTick t={elapsed:0.00} | "
                    + $"Pos={_stateManager.transform.position:F1} "
                    + $"Dest={_fleeTarget:F1} "
                    + $"Phase={(_isReturning ? "RETURN" : "FLEE")} "
                    + $"RemDist={_agent.remainingDistance:0.00} "
                    + $"HasPath={_agent.hasPath} "
                    + $"Pending={_agent.pathPending}");

            if (!_isReturning)
            {
                // --- fleeing phase ---
                if (!_agent.pathPending && _agent.remainingDistance <= ArriveThreshold)
                {
                    _isReturning = true;
                    Vector3 herdCenter = _stateManager.BehaviorContext.PlayerPosition;
                    Debug.Log($"[{_stateManager.name}] FLEE PHASE COMPLETE at {elapsed:0.0}s, returning to herd...");

                    if (NavMesh.SamplePosition(herdCenter, out NavMeshHit hit, 30f, NavMesh.AllAreas))
                    {
                        _agent.SetDestination(hit.position);
                        _agent.speed = (_stateManager.Config?.BaseSpeed ?? 2f) * 1.6f;
                        Debug.Log($"[{_stateManager.name}] Return destination set to {hit.position}");
                    }
                    else
                    {
                        Debug.LogWarning($"[{_stateManager.name}] Could not find valid herd return NavMesh position!");
                    }
                }
            }
            else
            {
                // --- returning phase ---
                Vector3 herdCenter = _stateManager.BehaviorContext.PlayerPosition;
                float dist = Vector3.Distance(_stateManager.transform.position, herdCenter);
                if (!_agent.pathPending && dist <= ReturnDistance)
                {
                    Debug.Log($"[{_stateManager.name}] RETURN COMPLETE at {elapsed:0.0}s (dist={dist:0.0})");
                    EndPanic();
                }
            }
        }

        private void EndPanic()
        {
            if (!_stateManager.CanControlAgent()) return;

            _agent.isStopped = false;
            _agent.autoBraking = true;
            _agent.speed = _stateManager.Config?.BaseSpeed ?? 2f;

            _stateManager.UnlockStateFromExternal();
            _stateManager.EnableBehavior();

            _stateManager.SetState<SheepGrazeState>();
            Debug.Log($"[{_stateManager.name}] Panic ended — control returned to manager");
        }


        public void OnStop()
        {
            Debug.Log($"[{_stateManager.name}] OnStop() called for PanicState");
            if (_stateManager.CanControlAgent())
                _stateManager.Agent.isStopped = false;

            _stateManager.Animation?.SetState((int)SheepAnimState.Idle);
        }
    }

} 
