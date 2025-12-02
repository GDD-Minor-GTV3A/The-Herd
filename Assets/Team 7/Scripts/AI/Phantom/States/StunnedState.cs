using UnityEngine;

namespace Team_7.Scripts.AI.Phantom.States
{
    public class StunnedState : PhantomState
    {
        private float _stunStart;
        
        public StunnedState(PhantomStateManager manager, EnemyMovementController movement, PhantomStats stats, PhantomAnimatorController animator, AudioController audio)
            : base(manager, movement, stats, animator, audio) { }


        public override void OnStart()
        {
            _movement.Agent.isStopped = true;
            _manager.CancelCharging();
            _stunStart = Time.time;
        }

        public override void OnUpdate()
        {
            if (Time.time - _stunStart > _stats.stunDuration)
                _manager.StartWandering();
        }

        public override void OnStop()
        {
            _movement.Agent.isStopped = false;
        }
    }
}
