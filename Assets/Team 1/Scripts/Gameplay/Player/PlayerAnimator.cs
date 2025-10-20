using UnityEngine;
using Core.Shared;
using UnityEngine.Animations.Rigging;
using System;

namespace Gameplay.Player
{
    public class PlayerAnimator : AnimatorController
    {
        private PlayerAnimationConstraints animationConstrains;
        private readonly Transform root;


        private const string WalkingParam = "Walk";
        private const string WalkSpeedParam = "WalkSpeed";


        private readonly int handsLayerIndex;

        public PlayerAnimator(Animator animator,Transform root, PlayerAnimationConstraints constraints) : base(animator)
        {
            handsLayerIndex = _animator.GetLayerIndex("Hands Layer");
            this.root = root;
            animationConstrains = constraints;
            RemoveHands();
        }


        /// <summary>
        /// Sets walking animation state.
        /// </summary>
        /// <param name="walking">Is walking.</param>
        public void SetWalking(bool walking)
        {
            _animator.SetBool(WalkingParam, walking);
        }


        /// <summary>
        /// Changes walk speed animation multiplier.
        /// </summary>
        /// <param name="sprint"> Is player sprinting.</param>
        public void SetWalkSpeed(bool sprint)
        {
            if (sprint)
                _animator.SetFloat(WalkSpeedParam, 2f);
            else
                _animator.SetFloat(WalkSpeedParam, 1f);
        }


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
                animationConstrains.LookTarget.position = root.transform.position + root.transform.forward * 10;
            }
        }


        public void GetTool(ToolAnimationKeyPoints keyPoints)
        {
            var _handData = animationConstrains.RightHand.data;
            _handData.target = keyPoints.RightHandTarget;
            _handData.hint = keyPoints.RightHandHint;
            animationConstrains.RightHand.data = _handData;

            _handData = animationConstrains.LeftHand.data;
            _handData.target = keyPoints.LeftHandTarget;
            _handData.hint = keyPoints.LeftHandHint;
            animationConstrains.LeftHand.data = _handData;


            var _shouldersData = animationConstrains.ShoulderAim.data;
            var _targets = _shouldersData.sourceObjects;
            _targets.Add(new WeightedTransform(keyPoints.ShouldersTarget, 1));
            _shouldersData.sourceObjects = _targets;
            animationConstrains.ShoulderAim.data = _shouldersData;

            animationConstrains.RightHand.weight = 1;
            animationConstrains.LeftHand.weight = 1;
            animationConstrains.ShoulderAim.weight = 1;
        }


        public void RemoveHands()
        {
            var _handData = animationConstrains.RightHand.data;
            _handData.target = null;
            _handData.hint = null;
            animationConstrains.RightHand.data = _handData;

            _handData = animationConstrains.LeftHand.data;
            _handData.target = null;
            _handData.hint = null;
            animationConstrains.LeftHand.data = _handData;


            var _shouldersData = animationConstrains.ShoulderAim.data;
            var _targets = _shouldersData.sourceObjects;
            _targets.Clear();
            _shouldersData.sourceObjects = _targets;
            animationConstrains.ShoulderAim.data = _shouldersData;

            animationConstrains.RightHand.weight = 0;
            animationConstrains.LeftHand.weight = 0;
            animationConstrains.ShoulderAim.weight = 0;
        }


        public void RotateCharacterBody(Vector3 mouseWorldPosition)
        {
            Vector3 direction = mouseWorldPosition - root.position;
            direction.Normalize();
            float angle = Vector3.SignedAngle(root.forward, direction, root.up);

            if (angle <= -90f)
                root.Rotate(0, -90, 0);

            if (angle >= 90f)
                root.Rotate(0, 90, 0);

            if (Mathf.Round(root.rotation.eulerAngles.y) % 90 == 0)
                root.Rotate(0, 45, 0);


            animationConstrains.LookTarget.position = new Vector3(mouseWorldPosition.x, animationConstrains.LookTarget.position.y, mouseWorldPosition.z);
        }
    }


    [Serializable]
    public struct PlayerAnimationConstraints
    {
        public MultiAimConstraint BodyAim;
        public MultiAimConstraint HeadAim;
        public MultiAimConstraint ShoulderAim;
        public TwoBoneIKConstraint LeftHand;
        public TwoBoneIKConstraint RightHand;

        public Transform LookTarget;
    }
}

