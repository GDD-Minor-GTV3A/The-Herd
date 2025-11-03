using AI;
using AI.Drekavac;
using AI.Drekavac.States;
using Core;
using UnityEngine;

namespace Team_7.Scripts.AI.Drekavac.States
{
    /// <summary>
    ///     Enemy "Big" state � the Drekavac charges directly at the player and attacks repeatedly when close.
    /// </summary>
    public class BigState : DrekavacState
    {
        private float _attackTimer;
        private Transform _playerTransform;
        private float _attackInterval = 2f; // TODO: Move this to DrekavacStats if you want to tweak per-enemy

        public BigState(DrekavacStateManager manager, EnemyMovementController movement, DrekavacStats stats, DrekavacAnimatorController animator, AudioController audio)
            : base(manager, movement, stats, animator, audio)
        {
        }

        public override void OnStart()
        {
            _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
            _manager.transform.localScale *= 2f;
            _movement.SetMovementSpeed(_stats.bigChargeSpeed); // TODO: Add bigChargeSpeed to DrekavacStats
            _attackTimer = 0f;

            // Optional: Play roar sound on entering BigState
            if (_stats.snarlSound != null)
                _audio.PlayClip(_stats.snarlSound);
        }

        public override void OnUpdate()
        {
            if (_playerTransform == null)
                return;

            Vector3 playerPosition = _playerTransform.position;

            // Charge straight at the player
            _movement.MoveTo(playerPosition);
            _movement.LookAt(playerPosition);

            float distanceToPlayer = Vector3.Distance(_manager.transform.position, playerPosition);

            // When close enough to attack
            if (distanceToPlayer <= 1f)
            {
                _attackTimer += Time.deltaTime;
                if (_attackTimer >= _attackInterval)
                {
                    _attackTimer = 0f;

                    // TODO: Play attack animation here

                    // TODO: Deal damage to player here
                    Debug.Log("Bit player");
                    // Keep following the player while attacking (staying "on" them)
                    _manager.transform.position = playerPosition - _manager.transform.forward * 0.8f;
                }
            }

            // TODO: Detect if Drekavac has been hit by a bullet
        }

        public override void OnStop()
        {
            
        }
    }
}
