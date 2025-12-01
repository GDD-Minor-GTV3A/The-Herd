using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Gameplay.SanityEffects
{
    [CreateAssetMenu(fileName = "SanityEffectsConfig", menuName = "Configs/SanityEffects")]
    public class SanityEffectsConfig : ScriptableObject
    {
        [field: SerializeField, Tooltip("Volume profile for sanity effects")]
        public VolumeProfile VolumeProfile {  get; private set; }

        [field: Space(10), Header("     Chromatic Aberration        "), Space(10)]
        [field: SerializeField, Tooltip("Max intensity of aberration effect.")]
        public float AberrationMaxValue { get; private set; } = .7f;


        [field: Space(10), Header("     Visual effects weights      "), Space(10)]
        [field: SerializeField, Tooltip("Intensity of visual effects when player has perfect sanity.")]
        public float DefaultWeight { get; private set; } = 0f;

        [field: SerializeField, Tooltip("Intensity of visual effects when player on Fragile sanity level.")]
        public float FragileWeight { get; private set; } = .3f;

        [field: SerializeField, Tooltip("Intensity of visual effects when player on Unstable sanity level.")]
        public float UnstableWeight { get; private set; } = .6f;

        [field: SerializeField, Tooltip("Intensity of visual effects when player on Break Point sanity level.")]
        public float BreakingPointWeight { get; private set; } = 1f;

        [field: SerializeField, Tooltip("Intensity of visual effects when player on Death sanity level.")]
        public float DeathWeight { get; private set; } = 1f;



        [field: Space(10), Header("     Enemy Shadows Spawner      "), Space(10)]
        [field: SerializeField, Tooltip("Prefabs of all enemies shadows that can be spawned.")]
        public List<GameObject> PossibleEnemiesToSpawn { get; private set; }

        [field: Space(.5f), Header("Attempts")]
        [field: SerializeField, Tooltip("Time between attempts of spawner. In seconds.")]
        public float TimeBetweenSpawnerAttempts { get; private set; } = 10f;

        [field: SerializeField, Tooltip("Cooldown of spawner. In seconds.")]
        public float AttemptsCooldown { get; private set; } = 60f;

        [field: Space(.5f), Header("Spawn Chance")]
        [field: SerializeField, Tooltip("Minimum chance of spawn enemies on attempt. In Percentages.")]
        public float MinSpawnChance { get; private set; } = 5f;

        [field: SerializeField, Tooltip("Maximum chance of spawn enemies on attempt. In Percentages.")]
        public float MaxSpawnChance { get; private set; } = 20f;

        [field: Space(.5f), Header("Sanity edges")]
        [field: SerializeField, Tooltip("Percentage of sanity when enemies start to spawn.")]
        public float MaxSanityForSpawn { get; private set; } = 75f;

        [field: SerializeField, Tooltip("Percentage of sanity when enemies spawn the most often.")]
        public float MinSanityForSpawn { get; private set; } = 40f;

        [field: Space(.5f), Header("Enemies amount")]
        [field: SerializeField, Tooltip("Minimum amount of enemies to spawn per one attempt.")]
        public int MinEnemiesAmount { get; private set; } = 3;

        [field: SerializeField, Tooltip("Maximum amount of enemies to spawn per one attempt.")]
        public int MaxEnemiesAmount { get; private set; } = 5;

        [field: Space(.5f), Header("Spawn distance")]
        [field: SerializeField, Tooltip("Minimum distance from player to spawn enemy.")]
        public float MinSpawnDistance { get; private set; } = 10f;

        [field: SerializeField, Tooltip("Maximum distance from player to spawn enemy.")]
        public float MaxSpawnDistance { get; private set; } = 15f;

        [field: Space(.5f), Header("Enemies duration")]
        [field: SerializeField, Tooltip("Minimum life time of spawned enemy.")]
        public float MinEnemyDuration { get; private set; } = 3f;

        [field: SerializeField, Tooltip("Maximum life time of spawned enemy.")]
        public float MaxEnemyDuration { get; private set; } = 6f;
    }
}
