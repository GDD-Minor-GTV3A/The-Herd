using UnityEngine;
using Core.Shared.StateMachine;
using UnityEngine.AI;

namespace Core.AI.Sheep
{
    public sealed class SheepPanicState : IState
    {
        private readonly SheepStateManager sm;
        private NavMeshAgent Agent => sm.Agent;

        private Vector3 fleeTarget;
        private const float FleeDistance = 100f;

        public SheepPanicState(SheepStateManager ctx) => sm = ctx;

        public void OnStart()
        {
            if (!sm.CanControlAgent()) return;
            var agent = Agent;
            if (agent == null) return;

            var herd = Object.FindObjectOfType<SheepHerdController>();
            Vector3 herdCenter = herd ? herd.transform.position : sm.transform.position;
            Vector3 fleeDir = (sm.transform.position - herdCenter).normalized;
            if (fleeDir.sqrMagnitude < 0.01f)
                fleeDir = Random.insideUnitSphere;
            fleeDir.y = 0f;

            fleeTarget = sm.transform.position + fleeDir * FleeDistance;
            if (NavMesh.SamplePosition(fleeTarget, out var hit, 30f, NavMesh.AllAreas))
                fleeTarget = hit.position;

            agent.isStopped = false;
            agent.SetDestination(fleeTarget);

            sm.Animation?.SetState((int)SheepAnimState.Run);
            Debug.Log($"[{sm.name}] 🏃 Fleeing to {fleeTarget}");
        }

        public void OnUpdate() { }   // handled by controller timer
        public void OnStop() { }
    }
}
