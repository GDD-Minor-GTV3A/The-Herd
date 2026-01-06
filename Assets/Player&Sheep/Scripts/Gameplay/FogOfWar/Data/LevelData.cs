using UnityEngine;
using UnityEngine.Events;

namespace Gameplay.FogOfWar
{
    /// <summary>
    /// Data of the level. Has to be unique for each level.
    /// </summary>
    [CreateAssetMenu(fileName = "LevelData", menuName = "Data/LevelData")]
    public class LevelData : ScriptableObject
    {
        [Header("Map data")]
        [SerializeField, Tooltip("The highest point of the map. You can see the Gizmos if it's turned on in FogOfWarManager.")] 
        private float mapHighestPoint = 0f;

        [SerializeField, Tooltip("The lowest point of the map. You can see the Gizmos if it's turned on in FogOfWarManager.")] 
        private float mapLowestPoint = 0f;

        [SerializeField, Tooltip("The width of level map. You can see the Gizmos if it's turned on in FogOfWarManager.")] 
        private float mapWidth = 0f;

        [SerializeField, Tooltip("The length of level map. You can see the Gizmos if it's turned on in FogOfWarManager.")] 
        private float mapLength = 0f;


        /// <summary>
        /// The highest point of the map. You can see the Gizmos if it's turned on in FogOfWarManager.
        /// </summary>
        public float MapHighestPoint => mapHighestPoint;
        /// <summary>
        /// The lowest point of the map. You can see the Gizmos if it's turned on in FogOfWarManager.
        /// </summary>
        public float MapLowestPoint => mapLowestPoint;
        /// <summary>
        /// The width of level map. You can see the Gizmos if it's turned on in FogOfWarManager.
        /// </summary>
        public float MapWidth => mapWidth;
        /// <summary>
        /// The length of level map. You can see the Gizmos if it's turned on in FogOfWarManager.
        /// </summary>
        public float MapLength => mapLength;


        /// <summary>
        /// Invokes when any values changes.
        /// </summary>
        public event UnityAction<LevelData> OnValueChanged;


        private void OnValidate()
        {
            if (mapHighestPoint < mapLowestPoint)
                mapHighestPoint = mapLowestPoint;


            OnValueChanged?.Invoke(this);
        }
    }
}