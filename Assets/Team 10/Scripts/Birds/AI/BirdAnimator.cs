using UnityEngine;
using System.Collections.Generic;

namespace Birds.AI
{
    /// <summary>
    /// Handles animation logic for the entire flock.
    /// Follows the AnimatorController pattern from the project architecture.
    /// </summary>
    public class BirdAnimator : MonoBehaviour
    {
        [Header("Settings")]
        [Tooltip("Parameter name for flying state")]
        [SerializeField] private string flyingBoolName = "IsFlying";

        // Internal reference to all animators (one per bird)
        private Animator[] animators;
        private int flyingBoolID;

        public void Initialize()
        {
            // Cache the Hash ID for performance (faster than using strings)
            flyingBoolID = Animator.StringToHash(flyingBoolName);

            // Find all animators on child objects (the actual bird models)
            animators = GetComponentsInChildren<Animator>();

            if (animators.Length == 0)
            {
                Debug.LogWarning("BirdAnimator: No Animators found in children!");
            }
        }

        /// <summary>
        /// Sets the flying state for all birds in the flock.
        /// </summary>
        public void SetFlying(bool isFlying)
        {
            if (animators == null) return;

            foreach (var anim in animators)
            {
                if (anim != null)
                {
                    anim.SetBool(flyingBoolID, isFlying);
                }
            }
        }
    }
}