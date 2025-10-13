using UnityEngine;

namespace Team_7.Scripts.AI.Phantom.States
{
    public class ShootingState : PhantomState
    {
        private float _chargeStart;
        private PhantomProjectile projectile;
        private float _lastShot;

        public ShootingState(PhantomStateManager manager, EnemyMovementController movement, PhantomStats stats, PhantomAnimatorController animator, AudioController audio) : base(manager, movement, stats, animator, audio)
        {
        }
        
        public override void OnStart()
        {
            if (_stats.shootCooldown > Time.time - _lastShot && _lastShot != 0)
            {
                _manager.SetState<WanderingState>();
                return;
            }
            
            _movement.Agent.ResetPath();
            _animator.SetCharging(true);
            projectile = _manager.StartCharging();
            _chargeStart = Time.time;
        }

        public override void OnUpdate()
        {
            _movement.LookAt(_manager.GetPlayerTransform().position);
            if (Time.time - _chargeStart > _stats.chargeDuration && !projectile.IsLaunched())
            {
                projectile.Launch();
                _animator.SetThrowing(true);
                _animator.SetCharging(false);
                _lastShot = Time.time;
            }
            
            // Check if the throwing animation has finished
            if (_animator.IsThrowingFinished()) 
                _manager.SetState<WanderingState>();
        }

        public override void OnStop()
        {
            _manager.CancelCharging();
            _chargeStart = 0;
            _animator.SetCharging(false);
            _animator.SetThrowing(false);
        }
    }
}
