using UnityEngine;

namespace Gameplay.FogOfWar
{
    [CreateAssetMenu(fileName = "SheepFogRevealerConfig", menuName = "Configs/Revealers/SheepFogRevealerConfig")]
    public class SheepFogRevealerConfig : FogRevealerConfig
    {
        [SerializeField] private AnimationCurve distanceFadingCurve;


        public AnimationCurve DistanceFadingCurve => distanceFadingCurve;

    }
}
