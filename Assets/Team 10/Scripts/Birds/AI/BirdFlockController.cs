using UnityEngine;
// using Core.Events; // Uncomment if you need the Event system later

namespace Birds.AI
{
    public class BirdFlockController : MonoBehaviour
    {
        [Header("Configuration")]
        [Tooltip("Reference to the Bird Config ScriptableObject")]
        [SerializeField] private BirdConfig config;

        [Header("References")]
        [Tooltip("The Transform holding the sphere models")]
        [SerializeField] private Transform visualRoot;
        
        // State Management
        private BirdState currentState;

        // Exposed properties for States to use
        public Transform VisualRoot => visualRoot;
        public Transform PlayerTransform { get; private set; }

        public BirdAnimator Animator { get; private set; } 

        private void Awake()
        {
            // Get the reference
            Animator = GetComponent<BirdAnimator>();
        }

        private void Start()
        {
            // Initialize the Animator wrapper
            Animator.Initialize();

            if (config == null) { /* ... error handling ... */ }

            // Start in Idle
            SetState(new BirdIdleState(this, config));
        }

        private void Update()
        {
            currentState?.Update();
        }

        public void SetState(BirdState newState)
        {
            currentState?.Exit();
            currentState = newState;
            currentState.Enter();
        }

        public void SetVisualsActive(bool isActive)
        {
            visualRoot.gameObject.SetActive(isActive);
        }

        private void OnTriggerEnter(Collider other)
        {
            // Only scare if it is the Player and we are currently Idle
            if ((other.CompareTag("Player") || other.CompareTag("Dog")) && currentState is BirdIdleState)
            {
                PlayerTransform = other.transform;
                SetState(new BirdFleeState(this, config));
            }
        }
    }
}