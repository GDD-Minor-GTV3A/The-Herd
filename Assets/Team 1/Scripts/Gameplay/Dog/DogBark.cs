using Core.Events;
using UnityEngine;
using UnityEngine.Events;

namespace Gameplay.Dog
{
    /// <summary>
    /// Handles logic of dog barking.
    /// </summary>
    [RequireComponent(typeof(Dog))]
    public class DogBark : MonoBehaviour
    {
        [Header("Debug")]
        [SerializeField, Tooltip("Turns on and of debug visuals.")] 
        private bool drawBarkArea = false;

        /// <summary>
        /// Invokes when dog barks.
        /// </summary>
        public UnityEvent onDogBark;


        private DogConfig config;
        private float lastBarkTime;
        private DogStateManager manager;


        /// <summary>
        /// Initialization method.
        /// </summary>
        /// <param name="config">Config of the dog.</param>
        public void Initialize(DogConfig config, DogStateManager manager)
        {
            this.manager = manager;
            this.config = config;
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

        /// <summary>
        /// If possible to bark, scares all scarables around the dog.
        /// </summary>
        public void TryBark()
        {
            if (manager.HerdZone.IsFreeSheepToHeard())
            {
                Debug.Log("Found free sheep");
                manager.SetState<DogMoveToSheep>();
            }

            if (Time.time - lastBarkTime < config.BarkCooldown)
                return;

            lastBarkTime = Time.time;

            onDogBark?.Invoke(); // trigger animation/sound via UnityEvent

            Collider[] _hits = Physics.OverlapSphere(transform.position, config.MaxBarkDistance, config.ScareableMask);

            foreach (Collider hit in _hits)
            {
                Vector3 _toTarget = (hit.transform.position - transform.position).normalized;

                float _angle = Vector3.Angle(transform.forward, _toTarget);
                if (_angle <= config.BarkAngle * 0.5f || config.BarkAngle >= 360f)
                {
                    if (hit.TryGetComponent(out IScareable scareable))
                    {
                        float _distance = Vector3.Distance(transform.position, hit.transform.position);
                        float _intensity = Mathf.Clamp01(1f - (_distance / config.MaxBarkDistance));
                        scareable.OnScared(transform.position, _intensity, ScareType.DogBark);
                    }
                }
            }

            if (drawBarkArea)
                DrawDebugBarkZone();
        }


        private void DrawDebugBarkZone()
        {
            Vector3 _leftLimit = Quaternion.Euler(0, -config.BarkAngle * 0.5f, 0) * transform.forward;
            Vector3 _rightLimit = Quaternion.Euler(0, config.BarkAngle * 0.5f, 0) * transform.forward;

            Debug.DrawLine(transform.position, transform.position + _leftLimit * config.MaxBarkDistance, Color.yellow, 1f);
            Debug.DrawLine(transform.position, transform.position + _rightLimit * config.MaxBarkDistance, Color.yellow, 1f);
            Debug.DrawLine(transform.position, transform.position + transform.forward * config.MaxBarkDistance, Color.cyan, 1f);
        }
    }
}