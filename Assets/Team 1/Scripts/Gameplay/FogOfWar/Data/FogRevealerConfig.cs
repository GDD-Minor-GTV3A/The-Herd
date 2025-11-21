using CustomEditor.Attributes;
using UnityEngine;
using UnityEngine.Events;

namespace Gameplay.FogOfWar 
{
    /// <summary>
    /// Config for revealer of Fog Of War.
    /// </summary>
    [CreateAssetMenu(fileName = "FogRevealerConfig", menuName = "Configs/FogRevealerConfig")]
    public class FogRevealerConfig : ScriptableObject
    {
        [SerializeField, Range(10f, 361f), Tooltip("Angle of view in degrees.")] private float fov = 90f;
        [SerializeField, Tooltip("Distance of view.")] private float viewDistance = 10f;
        [SerializeField, Tooltip("Quality of field of view shape. Affects performance!")] private uint rayCount = 50;
        [SerializeField, Tooltip("True if you want to update this revealer regularly, false if you don't want this revealer to be updated.")] private bool isStatic = true;
        [SerializeField, ShowIf("isStatic", true), Tooltip("How often filed of view updates. Less value - more frequent updates. 0 - updates every frame.")] private float updateRate = 10f;


        /// <summary>
        /// Angle of view in degrees.
        /// </summary>
        public float FOV => fov;
        /// <summary>
        /// Distance of view.
        /// </summary>
        public float ViewDistance => viewDistance;
        /// <summary>
        /// Quality of field of view shape. Affects performance!
        /// </summary>
        public uint RayCount => rayCount;
        /// <summary>
        /// True if you want to update this revealer regularly, false if you don't want this revealer to be updated.
        /// </summary>
        public bool IsStatic => isStatic;
        /// <summary>
        /// How often filed of view updates. Less value - more frequent updates. 0 - updates every frame.
        /// </summary>
        public float UpdateRate => updateRate;


        /// <summary>
        /// Invokes when any values changes.
        /// </summary>
        public event UnityAction<FogRevealerConfig> OnValueChanged;


        private void OnValidate()
        {
            if (viewDistance < 1f)
                viewDistance = 1f;

            if (updateRate < 0f)
                updateRate = 0;

            if (rayCount < 3)
                rayCount = 3;

            OnValueChanged?.Invoke(this);
        }
    }
}