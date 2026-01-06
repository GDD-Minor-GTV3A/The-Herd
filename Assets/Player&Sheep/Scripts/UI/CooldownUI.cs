using System.Collections;
using Core.Shared.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    /// Handles logic of cooldown icon.
    /// </summary>
    public class CooldownUI : MonoBehaviour
    {
        [SerializeField,Tooltip("Slider which represents cooldown value"), Required] 
        private Slider cooldownSlider;

        [SerializeField, Tooltip("Defines if cooldown stops when item is not equiped.")] 
        private bool stopOnDisable = true;


        private float duration;
        private float disableTime = 0f;
        private float remainingTime = 0f;
        private Coroutine cooldownCoroutine;


        /// <summary>
        /// Initialization method.
        /// </summary>
        /// <param name="duration">Duration of cooldown.</param>
        public void Initialize(float duration)
        {
            this.duration = duration;
            SetCooldownVisible(false);
        }


        /// <summary>
        /// Starts cooldown.
        /// </summary>
        public void StartCooldown()
        {
            if (cooldownSlider == null || duration <= 0f) return;

            SetCooldownVisible(true);
            SetCooldownProgress(1f);

            if (cooldownCoroutine != null) StopCoroutine(cooldownCoroutine);
            cooldownCoroutine = StartCoroutine(CooldownRoutine(duration));
        }

        private IEnumerator CooldownRoutine(float duration)
        {
            remainingTime = duration;
            while (remainingTime > 0)
            {
                remainingTime -= Time.deltaTime;
                float _remaining = Mathf.Clamp01(remainingTime / this.duration); // full -> empty
                SetCooldownProgress(_remaining);
                yield return null;
            }

            SetCooldownProgress(0f);
            SetCooldownVisible(false);
            cooldownCoroutine = null;
        }


        private void SetCooldownProgress(float v)
        {
            if (cooldownSlider != null)
                cooldownSlider.value = v;
        }

        private void SetCooldownVisible(bool visible)
        {
            if (cooldownSlider != null)
                cooldownSlider.gameObject.SetActive(visible);
        }


        private void OnDisable()
        {
            SetCooldownVisible(false);

            if (!stopOnDisable)
            {
                disableTime = Time.time;
            }
        }


        private void OnEnable()
        {
            if (!stopOnDisable)
            {
                float remainingDuration = remainingTime - (Time.time - disableTime);

                if (remainingDuration > 0)
                {
                    SetCooldownVisible(true);

                    cooldownCoroutine = StartCoroutine(CooldownRoutine(remainingDuration));
                }
            }
        }
    }
}