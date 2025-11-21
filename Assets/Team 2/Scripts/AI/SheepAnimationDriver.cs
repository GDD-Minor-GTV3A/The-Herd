using System;
using UnityEngine;

using Random = UnityEngine.Random;

namespace Core.AI.Sheep
{
    [RequireComponent(typeof(Animator))]
    public sealed class SheepAnimationDriver : MonoBehaviour
    {
        static readonly int HashState = Animator.StringToHash("State");
        static readonly int HashSpeed = Animator.StringToHash("Speed");

        [Header("Idle Variations Weights")] [SerializeField]
        private string _idleA = "IdleA_W";
        [SerializeField] private string _idleB = "IdleB_W";
        [SerializeField] private string _idleC = "IdleC_W";
        [SerializeField] private string _idleD = "IdleD_W";
        
        [Header("Jump and Run Parameters")]
        [SerializeField] private string _jumpTrigger = "Jump";
        [SerializeField] private float _runThreshold = 7f;
        [SerializeField] private float _jumpInterval = 1f;
        [SerializeField] private Vector2 _jumpJitter = new Vector2(-0.2f, 0.3f);
        //static readonly int HashIdleVariant = Animator.StringToHash("IdleVariant");
        //static readonly int HashGrazeNibble = Animator.StringToHash("GrazeNibble");

        [Header("Timing")] [SerializeField] private float _idleInterval = 5f;

        [SerializeField] private float _idleSpeed = 0.9f;
        [SerializeField] private Animator _animator;

        private int _speedHash, _jumpHash;
        private int _aHash, _bHash, _cHash, _dHash;
        private float _nextSwitchTime;
        private float nextJumpTime;
        private int currentIdle = -1;

        private void Awake()
        {
            _jumpHash = Animator.StringToHash(_jumpTrigger);
            _aHash = Animator.StringToHash(_idleA);
            _bHash = Animator.StringToHash(_idleB);
            _cHash = Animator.StringToHash(_idleC);
            _dHash = Animator.StringToHash(_idleD);

            ScheduleNextJump();
        }


        private void Update()
        {
            float speed = GetSpeed();

            if (speed >= _runThreshold && Time.time >= nextJumpTime && !IsInJump())
            {

                _animator.SetTrigger(_jumpHash);
                ScheduleNextJump();
            }
            if (speed <= _idleSpeed)
            {
                if (Time.time >= _nextSwitchTime)
                {
                    int idle = Random.Range(0, 4);
                    SetOneIdle(idle);
                    ScheduleIdle();
                }
            }
            if (speed < _runThreshold) ScheduleNextJump();
        }

        private void Reset()
        {
            _animator = GetComponent<Animator>();
        }

        public void SetState(int s)
        {
            if (_animator)
            {
                _animator.SetInteger(HashState, s);
            }
        }

        public void SetSpeed(float v)
        {
            if (_animator)
            {
                _animator.SetFloat(HashSpeed, v);
            }
        }

        private void SetOneIdle(int idx)
        {
            currentIdle = idx;
            
            _animator.SetFloat(_aHash, idx == 0 ? 1f : 0f);
            _animator.SetFloat(_bHash, idx == 1 ? 1f : 0f);
            _animator.SetFloat(_cHash, idx == 2 ? 1f : 0f);
            _animator.SetFloat(_dHash, idx == 3 ? 1f : 0f);
        }
        
        private void ScheduleNextJump()
        {
            float jitter = Random.Range(_jumpJitter.x, _jumpJitter.y);
            nextJumpTime = Time.time + Mathf.Max(0.1f, _jumpInterval + jitter);
        }

        private float GetSpeed()
        {
            return _animator.GetFloat(HashSpeed);
        }

        private void ScheduleIdle()
        {
            _nextSwitchTime = Time.time + _idleInterval;
        }

        private bool IsInJump()
        {
            var info = _animator.GetCurrentAnimatorStateInfo(0);
            return info.tagHash == Animator.StringToHash("Jump");
        }
        public void ApplyOverrideController(AnimatorOverrideController overrideController)
        {
            if(_animator && overrideController)
            {
                _animator.runtimeAnimatorController = overrideController;
            }
        }
    }

}
