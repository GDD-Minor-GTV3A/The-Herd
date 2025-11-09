using Core.Shared;
using UnityEngine;

namespace Gameplay.Dog 
{
    /// <summary>
    /// Handles animations logic for the dog.
    /// </summary>
    public class DogAnimator : AnimatorController
    {
        private const string  WalkingParam = "Walk";
        private const string WalkSpeedParam = "WalkSpeed";

        private float _minSpeed;
        private float _maxSpeed;

        private readonly float _minAnimationSpeed = .5f;
        private readonly float _maxAnimationSpeed = 2f;


        /// <param name="animator">Animator of the dog.</param>
        /// <param name="config">config of the dog.</param>
        public DogAnimator(Animator animator, DogConfig config) : base(animator)
        {
            UpdateAnimationValues(config);
        }


        /// <summary>
        /// Sets walking animation state.
        /// </summary>
        public void SetWalking(bool isWalking)
        {
            _animator.SetBool(WalkingParam, isWalking);
        }


        /// <summary>
        /// Calculates walking animation speed multiplier.
        /// </summary>
        /// <param name="currentSpeed">Current walking speed of the dog.</param>
        public void CalculateWalkingSpeedMultiplier(float currentSpeed)
        {
            float animSpeed = Mathf.Lerp(_minAnimationSpeed,_maxAnimationSpeed,Mathf.InverseLerp(_minSpeed, _maxSpeed, currentSpeed));
            SetWakingAnimationSpeedMulti(animSpeed);
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
            _minSpeed = config.MinSpeed;
            _maxSpeed = config.MaxSpeed;
        }
    }
}