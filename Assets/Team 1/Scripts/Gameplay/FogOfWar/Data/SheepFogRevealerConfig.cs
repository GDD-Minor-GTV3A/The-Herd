using UnityEngine;

namespace Gameplay.FogOfWar
{
    /// <summary>
    /// Config for revealer of Fog Of War. Made specifically for sheep fog revealers.
    /// </summary>
    [CreateAssetMenu(fileName = "SheepFogRevealerConfig", menuName = "Configs/Revealers/SheepFogRevealerConfig")]
    public class SheepFogRevealerConfig : FogRevealerConfig
    {
        [SerializeField, Tooltip("Curve represents dependency of view distance to the herd leader.")] 
        private AnimationCurve distanceFadingCurve;


        /// <summary>
        /// Curve represents dependency of view distance to the herd leader.
        /// </summary>
        public AnimationCurve DistanceFadingCurve => distanceFadingCurve;
    }
}