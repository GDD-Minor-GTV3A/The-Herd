using Core.Shared.Utilities;

using UnityEngine;
using UnityEngine.Events;

namespace Gameplay.FogOfWar 
{
    [CreateAssetMenu(fileName = "FogOfWarConfig", menuName = "Configs/FogOfWarConfig")]
    public class FogOfWarConfig : ScriptableObject
    {
        [Header("Utilities")]
        [SerializeField, Tooltip("")] private float _fogPlaneSize = 1f;
        [SerializeField, Tooltip("")] private uint _textureResolution = 100;
        [SerializeField, Tooltip("")] private LayerMask _obstaclesLayers;

        [Space]
        [Header("Materials")]
        [SerializeField, Required, Tooltip("")] private Material _fogProjectionMaterial;
        [SerializeField, Required, Tooltip("")] private Material _revealerMaterial;
        [SerializeField, Required, Tooltip("")] private Material _decalMaterial;
        [SerializeField, Required, Tooltip("")] private Material _fogMaterial;


        public float FogPlaneSize => _fogPlaneSize;
        public uint TextureResolution => _textureResolution;
        public LayerMask ObstaclesLayerMask => _obstaclesLayers;

        public Material FogProjectionMaterial => _fogProjectionMaterial;
        public Material RevealerMaterial => _revealerMaterial;
        public Material DecalMaterial => _decalMaterial;
        public Material FogMaterial => _fogMaterial;



        public event UnityAction<FogOfWarConfig> OnValueChanged;

        private void OnValidate()
        {
            if (_fogPlaneSize < 1)
            {
                _fogPlaneSize = 1;
            }

            if (_textureResolution < 100)
            {
                _textureResolution = 100;
            }


            OnValueChanged?.Invoke(this);
        }
    }
}