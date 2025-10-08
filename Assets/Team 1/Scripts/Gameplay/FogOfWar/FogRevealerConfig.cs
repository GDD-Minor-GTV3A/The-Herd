using UnityEngine;
using UnityEngine.Events;

namespace Gameplay.FogOfWar 
{
    [CreateAssetMenu(fileName = "FogRevealerConfig", menuName = "Configs/FogRevealerConfig")]
    public class FogRevealerConfig : ScriptableObject
    {
        [SerializeField, Range(10f, 360f), Tooltip("")] private float _fov = 90f;
        [SerializeField, Tooltip("")] private float _viewDistance = 10f;
        [SerializeField, Tooltip("")] private float _updateRate = 10f;
        [SerializeField, Tooltip("")] private uint _rayCount = 50;


        public float FOV => _fov;
        public float ViewDistance => _viewDistance;
        public float UpdateRate => _updateRate;
        public uint RayCount => _rayCount;


        public event UnityAction<FogRevealerConfig> OnValueChanged;

        private void OnValidate()
        {
            if (_viewDistance < 1f)
                _viewDistance = 1f;

            if (_updateRate < 0f)
                _updateRate = 0;


            OnValueChanged?.Invoke(this);
        }
    }
}