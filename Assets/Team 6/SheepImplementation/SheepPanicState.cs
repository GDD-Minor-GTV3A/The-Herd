using UnityEngine;
using Core.Shared.StateMachine;
using UnityEngine.AI;

namespace Core.AI.Sheep
{
    public sealed class SheepPanicState : IState
    {
        private readonly SheepStateManager stateManager;
        private NavMeshAgent Agent => stateManager.Agent;

        private Vector3 fleeTarget;
        private const float FleeDistance = 100f;

        public SheepPanicState(SheepStateManager ctx) => stateManager = ctx;

        public void OnStart()
        {
            if (!stateManager.CanControlAgent()) return;
            var agent = Agent;
            if (agent == null) return;

            var herd = Object.FindObjectOfType<SheepHerdController>();
            Vector3 herdCenter = herd ? herd.transform.position : stateManager.transform.position;
            Vector3 fleeDir = (stateManager.transform.position - herdCenter).normalized;
            if (fleeDir.sqrMagnitude < 0.01f)
                fleeDir = Random.insideUnitSphere;
            fleeDir.y = 0f;

            fleeTarget = stateManager.transform.position + fleeDir * FleeDistance;
            if (NavMesh.SamplePosition(fleeTarget, out var hit, 30f, NavMesh.AllAreas))
                fleeTarget = hit.position;

            agent.isStopped = false;
            agent.SetDestination(fleeTarget);

            //stateManager.Animation?.SetState((int)SheepAnimState.Run);
            Debug.Log($"[{stateManager.name}] 🏃 Fleeing to {fleeTarget}");
        }

        public void OnUpdate() { }   // handled by controller timer
        public void OnStop() { }
    }
}
