using System;
using System.Collections;

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

        private readonly Player player;
        private readonly Transform root;

        private readonly float startYRotation;


        private const string WalkingX = "X";
        private const string WalkingY = "Y";


        public PlayerAnimator(Animator animator, Player player, Transform root, PlayerAnimationConstraints constraints) : base(animator)
        {
            this.root = root;
            this.player = player;
            animationConstrains = constraints;
            startYRotation = root.eulerAngles.y;

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
        public void Walking(Vector2 walkingDirection, bool sprint = false)
        {
            float _x = walkingDirection.x;
            float _y = walkingDirection.y;

            _x /= (sprint) ? 1 : 2;
            _y /= (sprint) ? 1 : 2;

            float _yRotation = root.eulerAngles.y;

            int _roundYRotation = Mathf.RoundToInt(_yRotation - startYRotation);

            if (_roundYRotation != 0)
            {
                float _tempX = _x;
                float _tempY = _y;

                switch (Mathf.Abs(_roundYRotation / 90))
                {
                    case 1:
                        _x = -_tempY;
                        _y = _tempX;
                        break;
                    case 2:
                        _x *= -1;
                        _y *= -1;
                        break;
                    case 3:
                        _x = _tempY;
                        _y = -_tempX;
                        break;
                }
            }

            if (sprint)
            {
                _x = 0;
                _y = 1;
            }

            _animator.SetFloat(WalkingX, _x);
            _animator.SetFloat(WalkingY, _y);
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
            Walking(Vector2.zero);
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