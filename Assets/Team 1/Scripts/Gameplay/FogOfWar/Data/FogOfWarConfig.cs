using Core.Shared.Utilities;
using UnityEngine;
using UnityEngine.Events;

namespace Gameplay.FogOfWar 
{
    /// <summary>
    /// General config for Fog Of War. Can be unique for each level.
    /// </summary>
    [CreateAssetMenu(fileName = "FogOfWarConfig", menuName = "Configs/FogOfWarConfig")]
    public class FogOfWarConfig : ScriptableObject
    {
        [Header("Utilities")]
        [SerializeField, Tooltip("The size of Fog Of War Effect. You can see the Gizmos if it's turned on in FogOfWarManager.")] 
        private float fogPlaneSize = 1f;

        [SerializeField, Tooltip("The resolution of Fog Of War Texture. Affects performance! Don't make it 0!")] 
        private uint textureResolution = 100;

        [SerializeField,Required, Tooltip("Shader, which calculates visibility of hidden objects.")] 
        private ComputeShader computeShaderForHiddenObjects;

        [SerializeField, Tooltip("Layers, which blocks view.")] 
        private LayerMask obstaclesLayers;

        [Space, Header("Materials")]
        [SerializeField, Required, Tooltip("Material for Fog Of War projection plane.")] 
        private Material fogProjectionMaterial;

        [SerializeField, Required, Tooltip("Material for revealers meshes.")] 
        private Material revealerMaterial;

        [SerializeField, Required, Tooltip("Material for Fog Of War decal effect.")] 
        private Material fogEffectMaterial;


        /// <summary>
        /// The size of Fog Of War Effect. You can see the Gizmos if it's turned on in FogOfWarManager.
        /// </summary>
        public float FogPlaneSize => fogPlaneSize;
        /// <summary>
        /// The resolution of Fog Of War Texture. Affect performance! Don't make it 0!
        /// </summary>
        public uint TextureResolution => textureResolution;
        /// <summary>
        /// Shader, which calculates visibility of hidden objects.
        /// </summary>
        public ComputeShader ComputeShader => computeShaderForHiddenObjects;
        /// <summary>
        /// Layers, which blocks view.
        /// </summary>
        public LayerMask ObstaclesLayerMask => obstaclesLayers;

        /// <summary>
        /// Material for Fog Of War projection plane.
        /// </summary>
        public Material FogProjectionMaterial => fogProjectionMaterial;
        /// <summary>
        /// Material for revealers meshes.
        /// </summary>
        public Material RevealerMaterial => revealerMaterial;
        /// <summary>
        /// Material for Fog Of War material effect.
        /// </summary>
        public Material FogMaterial => fogEffectMaterial;


        /// <summary>
        /// Invokes when any value changes.
        /// </summary>
        public event UnityAction<FogOfWarConfig> OnValueChanged;


        private void OnValidate()
        {
            if (fogPlaneSize < 1)
            {
                fogPlaneSize = 1;
            }

            if (textureResolution < 0)
            {
                textureResolution = 0;
            }

            if (!fogEffectMaterial.shader.name.Contains("Decal"))
            {
                Debug.LogWarning("Material you are trying to assign for fog of war effect inherits from wrong shader!!!");
                fogEffectMaterial = null;
            }

            OnValueChanged?.Invoke(this);
        }
    }
}