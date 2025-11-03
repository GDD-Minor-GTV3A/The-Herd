using AI;
using AI.Phantom;
using AI.Phantom.States;

using Core;

using Phantom;

namespace Team_7.Scripts.AI.Phantom.States
{
    public class StunnedState : PhantomState
    {
        public StunnedState(PhantomStateManager manager, EnemyMovementController movement, PhantomStats stats, PhantomAnimatorController animator, AudioController audio)
            : base(manager, movement, stats, animator, audio) { }


        public override void OnStart()
        {
            _movement.Agent.isStopped = true;
            _manager.CancelCharging();
        }

        public override void OnStop()
        {
            _movement.Agent.isStopped = false;
        }
    }
}
