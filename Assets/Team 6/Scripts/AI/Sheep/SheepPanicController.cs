using UnityEngine;
using Core.AI.Sheep;

namespace AI.Sheep.Panic
{
    [RequireComponent(typeof(SheepStateManager))]
    public class SheepPanicController : MonoBehaviour
    {
        [Header("Sanity Settings")]
        [SerializeField] private int sanity = 0;
        [SerializeField] private int maxSanity = 100;
        [SerializeField] private int panicThreshold = 20;

        [Header("Panic Behaviour")]
        [SerializeField] private float panicDuration = 3f;
        [SerializeField] private float panicSpeedMultiplier = 1.8f;
        [SerializeField] private bool debugLogs = true;

        private SheepStateManager stateManager;
        private bool isPanicking = false;
        private float panicTimer = 0f;
        private float originalSpeed;
        private bool herdDisabled = false;

        void Awake()
        {
            stateManager = GetComponent<SheepStateManager>();
            if (stateManager.Agent != null)
                originalSpeed = stateManager.Agent.speed;
        }

        void Update()
        {

        }

        private void EnterPanicState()
        {

        }

        private void ExitPanicState()
        {

        }
        
        private void GainSanity()
        {
            
        }
    }
}
