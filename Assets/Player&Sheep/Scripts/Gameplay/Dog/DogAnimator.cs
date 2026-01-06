using Core.Events;
using Core.Shared;
using UnityEngine;

namespace Gameplay.Dog 
{
    /// <summary>
    /// Handles animations logic for the dog.
    /// </summary>
    public class DogAnimator : AnimatorController, IPausable
    {
        private bool isWalking;

        private float minSpeed;
        private float maxSpeed;

        private readonly float minAnimationSpeed = .5f;
        private readonly float maxAnimationSpeed = 2f;

        private const string WalkingParam = "Walk";
        private const string WalkSpeedParam = "WalkSpeed";


        /// <param name="animator">Animator of the dog.</param>
        /// <param name="config">config of the dog.</param>
        public DogAnimator(Animator animator, DogConfig config) : base(animator)
        {
            UpdateAnimationValues(config);

            EventManager.Broadcast(new RegisterNewPausableEvent(this));
        }


        /// <summary>
        /// Sets walking animation state.
        /// </summary>
        public void SetWalking(bool isWalking)
        {
            this.isWalking = isWalking; 
            _animator.SetBool(WalkingParam, isWalking);
        }


        /// <summary>
        /// Calculates walking animation speed multiplier.
        /// </summary>
        /// <param name="currentSpeed">Current walking speed of the dog.</param>
        public void CalculateWalkingSpeedMultiplier(float currentSpeed)
        {
            float _animationSpeed = Mathf.Lerp(minAnimationSpeed,maxAnimationSpeed,Mathf.InverseLerp(minSpeed, maxSpeed, currentSpeed));
            SetWakingAnimationSpeedMulti(_animationSpeed);
        }


        /// <summary>
        /// Sets walking animation speed multiplier.
        /// </summary>
        /// <param name="multiplier">New value for multiplier.</param>
        public void SetWakingAnimationSpeedMulti(float multiplier)
        {
            _animator.SetFloat(WalkSpeedParam, multiplier);
        }


        /// <summary>
        /// Update values according to config.
        /// </summary>
        /// <param name="config">Config of the dog.</param>
        public void UpdateAnimationValues(DogConfig config)
        {
            minSpeed = config.MinSpeed;
            maxSpeed = config.MaxSpeed;
        }

        public void Pause()
        {
            _animator.SetBool(WalkingParam, false);
        }

        public void Resume()
        {
            _animator.SetBool(WalkingParam, isWalking);
        }
    }
}