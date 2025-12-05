using System;
using Core.Events;
using Core.Shared;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace Gameplay.Player
{
    public class PlayerAnimator : AnimatorController, IPausable
    {
        private readonly PlayerAnimationConstraints animationConstrains;

        private readonly Transform lookTarget;
        private readonly Transform rightHandTarget;
        private readonly Transform rightHandHint;
        private readonly Transform leftHandTarget;
        private readonly Transform leftHandHint;

        private readonly Transform root;

        private const string WalkingSpeed = "WalkingSpeed";


        public PlayerAnimator(Animator animator, Transform root, PlayerAnimationConstraints constraints) : base(animator)
        {
            this.root = root;
            animationConstrains = constraints;

            lookTarget = animationConstrains.HeadAim.data.sourceObjects[0].transform;

            rightHandTarget = animationConstrains.RightHand.data.target;
            rightHandHint = animationConstrains.RightHand.data.hint;

            leftHandTarget = animationConstrains.LeftHand.data.target;
            leftHandHint = animationConstrains.LeftHand.data.hint;

            animationConstrains.HeadAim.weight = 1;

            EventManager.Broadcast(new RegisterNewPausableEvent(this));
        }


        /// <summary>
        /// Sets walking animation state.
        /// </summary>
        /// <param name="walkingDirection">Direction of walking.</param>
        /// <param name="sprint">Is sprinting.</param>
        public void Walking(bool isWalking, bool sprint = false)
        {
            float wakingSpeed = isWalking ? 1 : 0;

            wakingSpeed /= (sprint) ? 1 : 2;

            _animator.SetFloat(WalkingSpeed, wakingSpeed);
        }


        /// <summary>
        /// Rotate character towards cursor world position.
        /// </summary>
        public void RotateCharacterBody(Vector3 mouseWorldPosition)
        {
            Vector3 direction = mouseWorldPosition - root.position;
            direction.Normalize();
            float angle = Vector3.SignedAngle(root.forward, direction, root.up);

            if (angle <= -70f)
                root.Rotate(0, -90, 0);

            if (angle >= 70f)
                root.Rotate(0, 90, 0);
        }


        public void Pause()
        {
            Walking(false);
            animationConstrains.HeadAim.weight = 0;
            lookTarget.position = root.transform.position + root.transform.forward * 10;
        }

        public void Resume()
        {
            animationConstrains.HeadAim.weight = 1;
        }


        public void RotateHead(Vector3 mouseWorldPosition)
        {
            Vector3 targetPosition = new Vector3(mouseWorldPosition.x, lookTarget.position.y, mouseWorldPosition.z);
            Vector3 playerPosition = root.position;

            Vector2 targetPositionXZ = new Vector2(mouseWorldPosition.x, mouseWorldPosition.z);
            Vector2 playerPositionXZ = new Vector2(root.position.x, root.position.z);

            float distance = Vector2.Distance(targetPositionXZ, playerPositionXZ);

            if (distance < 2.5f)
            {
                Vector2 correctionDirectionXZ = targetPositionXZ - playerPositionXZ;
                correctionDirectionXZ.Normalize();

                Vector3 correctionDirection = new Vector3(correctionDirectionXZ.x, 0, correctionDirectionXZ.y);

                targetPosition += correctionDirection * (2.5f - distance);
            }

            lookTarget.position = targetPosition;
        }
    }


    [Serializable]
    public struct PlayerAnimationConstraints
    {
        public MultiAimConstraint HeadAim;
        public TwoBoneIKConstraint LeftHand;
        public TwoBoneIKConstraint RightHand;
    }
}