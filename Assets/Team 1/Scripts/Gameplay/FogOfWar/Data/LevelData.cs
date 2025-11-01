using UnityEngine;
using UnityEngine.Events;

namespace Gameplay.FogOfWar
{
    /// <summary>
    /// Data of the level.
    /// </summary>
    [CreateAssetMenu(fileName = "LevelData", menuName = "Data/LevelData")]
    public class LevelData : ScriptableObject
    {
        [Header("Map data")]
        [SerializeField, Tooltip("The highest point of the map. You can see the Gizmos if it's turned on in FogOfWarManager.")] private float mapHighestPoint = 0f;
        [SerializeField, Tooltip("The lowest point of the map. You can see the Gizmos if it's turned on in FogOfWarManager.")] private float mapLowestPoint = 0f;


        /// <summary>
        /// The highest point of the map. You can see the Gizmos if it's turned on in FogOfWarManager.
        /// </summary>
        public float MapHighestPoint => mapHighestPoint;
        /// <summary>
        /// The lowest point of the map. You can see the Gizmos if it's turned on in FogOfWarManager.
        /// </summary>
        public float MapLowestPoint => mapLowestPoint;


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