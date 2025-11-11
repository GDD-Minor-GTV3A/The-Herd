using System.Collections;
using Core.Events;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Gameplay.Dog
{
    [RequireComponent(typeof(Dog))]
    public class DogBark : MonoBehaviour
    {
        [SerializeField] private UnityEvent onDogBark;

        [Header("UI")]
        [SerializeField] private Slider cooldownSlider;

        [Header("Debug")]
        [SerializeField] private bool drawBarkArea = false;

        private DogConfig _config;

        private float _lastBarkTime;

        private Coroutine _cooldownCo;

        public void Initialize(DogConfig config)
        {
            _config = config;
            SetCooldownVisible(false);
            SetCooldownProgress(0f);
        }

        private void Awake()
        {
            SetCooldownVisible(false);
            SetCooldownProgress(0f);
        }

        private void OnEnable()
        {
            EventManager.AddListener<DogBarkEvent>(OnBarkCommand);
            SetCooldownVisible(false);
        }

        private void OnDisable()
        {
            EventManager.RemoveListener<DogBarkEvent>(OnBarkCommand);

            if (_cooldownCo != null)
            {
                StopCoroutine(_cooldownCo);
                _cooldownCo = null;
            }
            SetCooldownVisible(false);
        }

        private void OnBarkCommand(DogBarkEvent evt)
        {
            TryBark();
        }

        public void TryBark()
        {
            if (Time.time - _lastBarkTime < _config.BarkCooldown)
                return;

            _lastBarkTime = Time.time;

            onDogBark?.Invoke(); // trigger animation/sound via UnityEvent

            StartCooldownUI(_config.BarkCooldown);

            Collider[] hits = Physics.OverlapSphere(transform.position, _config.MaxBarkDistance, _config.ScareableMask);

            foreach (Collider hit in hits)
            {
                Vector3 toTarget = (hit.transform.position - transform.position).normalized;

                float angle = Vector3.Angle(transform.forward, toTarget);
                if (angle <= _config.BarkAngle * 0.5f || _config.BarkAngle >= 360f)
                {
                    if (hit.TryGetComponent(out IScareable scareable))
                    {
                        float distance = Vector3.Distance(transform.position, hit.transform.position);
                        float intensity = Mathf.Clamp01(1f - (distance / _config.MaxBarkDistance));
                        scareable.OnScared(transform.position, intensity, ScareType.DogBark);
                    }
                }
            }

            if (drawBarkArea)
                DrawDebugBarkZone();
        }

        private void StartCooldownUI(float duration)
        {
            if (cooldownSlider == null || duration <= 0f) return;

            
            SetCooldownVisible(true);
            SetCooldownProgress(1f);

            if (_cooldownCo != null) StopCoroutine(_cooldownCo);
            _cooldownCo = StartCoroutine(CooldownRoutine(duration));
        }

        private IEnumerator CooldownRoutine(float duration)
        {
            float t = 0f;
            while (t < duration)
            {
                t += Time.deltaTime;
                float remaining = 1f - Mathf.Clamp01(t / duration); // full -> empty
                SetCooldownProgress(remaining);
                yield return null;
            }

            SetCooldownProgress(0f);
            SetCooldownVisible(false);
            _cooldownCo = null;
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


        private void DrawDebugBarkZone()
        {
            Vector3 leftLimit = Quaternion.Euler(0, -_config.BarkAngle * 0.5f, 0) * transform.forward;
            Vector3 rightLimit = Quaternion.Euler(0, _config.BarkAngle * 0.5f, 0) * transform.forward;

            Debug.DrawLine(transform.position, transform.position + leftLimit * _config.MaxBarkDistance, Color.yellow, 1f);
            Debug.DrawLine(transform.position, transform.position + rightLimit * _config.MaxBarkDistance, Color.yellow, 1f);
            Debug.DrawLine(transform.position, transform.position + transform.forward * _config.MaxBarkDistance, Color.cyan, 1f);
        }
    }
}
