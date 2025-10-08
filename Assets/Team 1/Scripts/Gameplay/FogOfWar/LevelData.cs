using UnityEngine;
using UnityEngine.Events;

namespace Gameplay.FogOfWar
{
    [CreateAssetMenu(fileName = "LevelData", menuName = "Data/LevelData")]
    public class LevelData : ScriptableObject
    {
        [Header("Map data")]
        [SerializeField, Tooltip("")] private float _mapHighestPoint = 0f;
        [SerializeField, Tooltip("")] private float _mapLowestPoint = 0f;


        public float MapHighestPoint => _mapHighestPoint;
        public float MapLowestPoint => _mapLowestPoint;


        public event UnityAction<LevelData> OnValueChanged;

        private void OnValidate()
        {
            if (_mapHighestPoint < _mapLowestPoint)
                _mapHighestPoint = _mapLowestPoint;


            OnValueChanged?.Invoke(this);
        }
    }
}