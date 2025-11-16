using System;
using System.Collections;
using Core.Shared;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace Gameplay.Player
{
    public class PlayerAnimator : AnimatorController
    {
        private readonly PlayerAnimationConstraints animationConstrains;

        private readonly Transform lookTarget;
        private readonly Transform rightHandTarget;
        private readonly Transform rightHandHint;
        private readonly Transform leftHandTarget;
        private readonly Transform leftHandHint;
        private readonly Transform shouldersTarget;

        private readonly Player player;
        private readonly Transform root;

        private readonly float startYRotation;

        private Coroutine rightHandCoroutine;
        private Coroutine leftHandCoroutine;
        private Coroutine shouldersCoroutine;


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

            shouldersTarget = animationConstrains.ChestAim.data.sourceObjects[0].transform;

            RemoveHands();
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
                        _x = _tempY;
                        _y = -_tempX;
                        break;
                    case 2:
                        _x *= -1;
                        _y *= -1;
                        break;
                    case 3:
                        _x = -_tempY;
                        _y = _tempX;
                        break;
                }
            }

            _animator.SetFloat(WalkingX, _x);
            _animator.SetFloat(WalkingY, _y);
        }


        /// <summary>
        /// Define if animator controls character rotation or no.
        /// </summary>
        public void SetAnimationRotation(bool rotate)
        {
            if (rotate)
            {
                animationConstrains.HeadAim.weight = 1;
                animationConstrains.BodyAim.weight = 1;
            }
            else
            {
                animationConstrains.HeadAim.weight = 0;
                animationConstrains.BodyAim.weight = 0;
                lookTarget.position = root.transform.position + root.transform.forward * 10;
            }
        }


        /// <summary>
        /// Moves hands on tool's animation key points.
        /// </summary>
        public void GetTool(ToolAnimationKeyPoints keyPoints)
        {
            TwoBoneIKConstraintData _handData;

            if (keyPoints.RightHandTarget != null)
            {
                animationConstrains.RightHand.weight = 1;

                if (keyPoints.RightHandHint != null)
                {
                    _handData = animationConstrains.RightHand.data;
                    _handData.hintWeight = 1;
                    animationConstrains.RightHand.data = _handData;
                }

                rightHandCoroutine = player.StartCoroutine(HandRoutine(rightHandTarget, rightHandHint, keyPoints.RightHandTarget, keyPoints.RightHandHint));

            }

            if (keyPoints.LeftHandTarget != null)
            {
                animationConstrains.LeftHand.weight = 1;

                if (keyPoints.LeftHandHint != null)
                {
                    _handData = animationConstrains.LeftHand.data;
                    _handData.hintWeight = 1;
                    animationConstrains.LeftHand.data = _handData;
                }

                leftHandCoroutine = player.StartCoroutine(HandRoutine(leftHandTarget, leftHandHint, keyPoints.LeftHandTarget, keyPoints.LeftHandHint));
            }



            if (keyPoints.ChestTarget != null)
            {
                animationConstrains.ChestAim.weight = 1;

                shouldersCoroutine = player.StartCoroutine(ShouldersRoutine(keyPoints.ChestTarget));
            }
        }


        private IEnumerator HandRoutine(Transform baseHandTarget, Transform baseHandHint, Transform newHandTarget, Transform newHandHint)
        {
            while (true)
            {
                baseHandTarget.position = newHandTarget.position;
                baseHandTarget.rotation = newHandTarget.rotation;
                if (newHandHint != null)
                {
                    baseHandHint.position = newHandHint.position;
                    baseHandHint.rotation = newHandHint.rotation;
                }

                yield return null;
            }
        }


        private IEnumerator ShouldersRoutine(Transform newShouldersTarget)
        {
            while (true)
            {
                shouldersTarget.position = newShouldersTarget.position;
                shouldersTarget.rotation = newShouldersTarget.rotation;

                yield return null;
            }
        }


        /// <summary>
        /// Resets hands position.
        /// </summary>
        public void RemoveHands()
        {
            TwoBoneIKConstraintData _handData;

            animationConstrains.RightHand.weight = 0;

            _handData = animationConstrains.RightHand.data;
            _handData.hintWeight = 0;
            animationConstrains.RightHand.data = _handData;

            if (rightHandCoroutine != null)
            {
                player.StopCoroutine(rightHandCoroutine);
                rightHandCoroutine = null;
            }

            animationConstrains.LeftHand.weight = 0;

            _handData = animationConstrains.LeftHand.data;
            _handData.hintWeight = 0;
            animationConstrains.LeftHand.data = _handData;

            if (leftHandCoroutine != null)
            {
                player.StopCoroutine(leftHandCoroutine);
                leftHandCoroutine = null;
            }

            animationConstrains.ChestAim.weight = 0;
            if (shouldersCoroutine != null)
            {
                player.StopCoroutine(shouldersCoroutine);
                shouldersCoroutine = null;
            }
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


            Vector3 targetPosition = new Vector3(mouseWorldPosition.x, lookTarget.position.y, mouseWorldPosition.z);

            lookTarget.position = targetPosition;
        }
    }


    [Serializable]
    public struct PlayerAnimationConstraints
    {
        public MultiAimConstraint BodyAim;
        public MultiAimConstraint HeadAim;
        public MultiAimConstraint ChestAim;
        public TwoBoneIKConstraint LeftHand;
        public TwoBoneIKConstraint RightHand;
    }
}