using Core.AI.Sheep.Event;
using Core.Events;
using Core.Shared.Utilities;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Gameplay.SanityEffects
{
    public class SanityEffectsManager : MonoBehaviour
    {
        [SerializeField]
        private SanityEffectsConfig config;

        [SerializeField, Required]
        private Transform playerTransform;

        private Volume volume;
        private DepthOfField blur;
        private EnemyShadowsSpawner shadowsSpawner;


        public void Initialize()
        {
            volume = gameObject.AddComponent<Volume>();

            volume.weight = config.DefaultWeight;
            volume.priority = 10f;
            volume.profile = config.VolumeProfile;


            ChromaticAberration chromaticAberration;


            if (!config.VolumeProfile.TryGet(out chromaticAberration))
            {
                chromaticAberration = config.VolumeProfile.Add<ChromaticAberration>();
            }
            chromaticAberration.intensity.overrideState = true;
            chromaticAberration.intensity.value = config.AberrationMaxValue;

            shadowsSpawner = gameObject.AddComponent<EnemyShadowsSpawner>();
            shadowsSpawner.Initialize(config, playerTransform);

            EventManager.AddListener<SanityChangeEvent>(UpdateSpawner);

        }


        private void UpdateSpawner(SanityChangeEvent evt)
        {
            if (evt.Percentage > config.MaxSanityForSpawn)
            {
                if (shadowsSpawner.IsActive)
                    shadowsSpawner.StopSpawner();
            }
            else
            {
                if (!shadowsSpawner.IsActive)
                    shadowsSpawner.StartSpawner();
                shadowsSpawner.UpdateCurrentChance(evt.Percentage);
            }

            volume.weight = 1 - evt.Percentage / 100;
        }


        private void UpdateVisualEffects(SanityStageChangeEvent evt)
        {
            switch (evt.NewStage)
            {
                case SanityStage.Fragile:
                    volume.weight = config.FragileWeight;
                    break;

                case SanityStage.Unstable:
                    volume.weight = config.UnstableWeight;
                    break;

                case SanityStage.BreakingPoint:
                    volume.weight = config.BreakingPointWeight;
                    break;

                case SanityStage.Death:
                    volume.weight = config.DeathWeight;
                    break;

                default:
                    volume.weight = config.DefaultWeight;
                    break;
            }
        }

        private void OnDestroy()
        {
            EventManager.RemoveListener<SanityStageChangeEvent>(UpdateVisualEffects);
        }
    }
}