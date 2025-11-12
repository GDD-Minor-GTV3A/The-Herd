using System.Collections;
using Core.Shared.Utilities;
using UnityEngine;
using UnityEngine.UI;

public class CooldownUI : MonoBehaviour
{
    [SerializeField, Required] private Slider cooldownSlider;
    [SerializeField] private bool stopOnDisable = true;


    private float _duration;
    private float disableTime = 0f;
    private float remainingTime = 0f;
    private Coroutine cooldownCo;


    public void Initialize(float duration)
    {
        this._duration = duration;
        SetCooldownVisible(false);
    }


    public void StartCooldown()
    {
        if (cooldownSlider == null || _duration <= 0f) return;

        SetCooldownVisible(true);
        SetCooldownProgress(1f);

        if (cooldownCo != null) StopCoroutine(cooldownCo);
        cooldownCo = StartCoroutine(CooldownRoutine(_duration));
    }


    private IEnumerator CooldownRoutine(float duration)
    {
        remainingTime = duration;
        while (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;
            float remaining = Mathf.Clamp01(remainingTime / _duration); // full -> empty
            SetCooldownProgress(remaining);
            yield return null;
        }

        SetCooldownProgress(0f);
        SetCooldownVisible(false);
        cooldownCo = null;
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
        if(!stopOnDisable)
        {
            float remainingDuration = remainingTime - (Time.time - disableTime);

            if (remainingDuration > 0)
            {
                SetCooldownVisible(true);

                cooldownCo = StartCoroutine(CooldownRoutine(remainingDuration));
            }
        }
    }
}