using UnityEngine;

namespace AI.Sheep.Panic
{
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

        
    }
}
