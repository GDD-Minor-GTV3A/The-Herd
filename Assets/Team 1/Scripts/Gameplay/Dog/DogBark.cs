using Core.Events;
using UnityEngine;
using UnityEngine.Events;

namespace Gameplay.Dog
{
    [RequireComponent(typeof(Dog))]
    public class DogBark : MonoBehaviour
    {
        [SerializeField] private UnityEvent onDogBark;

        [Header("Debug")]
        [SerializeField] private bool drawBarkArea = false;

        private DogConfig _config;

        private float _lastBarkTime;

        public void Initialize(DogConfig config)
        {
            _config = config;
        }

        private void OnEnable()
        {
            EventManager.AddListener<DogBarkEvent>(OnBarkCommand);
        }

        private void OnDisable()
        {
            EventManager.RemoveListener<DogBarkEvent>(OnBarkCommand);
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
