using UnityEngine;

#nullable enable // Chris: This needs to be added for nullable values to work. Otherwise it will show a warning
namespace Team_7.Scripts.AI.Drekavac
{
    /// <summary>
    ///     Holds all the parameters that a "Drekavac" type enemy will need to function.
    /// </summary>
    [CreateAssetMenu(fileName = "DrekavacStats", menuName = "Scriptable Objects/DrekavacStats")]
    public class DrekavacStats : ScriptableObject
    {
        [Header("Movement Settings")]
        public float circleRadius = 10f;
        public float moveSpeed = 7f;
        public float sprintSpeed = 15f;
        public float dragSpeed = 4f;
        public float circleSpeed = 7f;
        public float bigChargeSpeed = 20;

        [Header("Behavior Settings")]
        public float directionSwitchInterval = 5f;
        public Vector2 stalkDurationRange = new Vector2(10f, 20f);

        [Header("Grab Settings")]
        public float dragAwayDistance = 20f;
        public float despawnDistance = 30f;

        [Header("Flee Settings")]
        public float fleeDistance = 30f;
        public float fleeSpeed = 15f;
        public float fleeTriggerDistance = 5f;
    
        [Header("Audio Settings")]
        public AudioClip screechSound;
        public AudioClip chompSound;
        public AudioClip snarlSound;
    }
}
