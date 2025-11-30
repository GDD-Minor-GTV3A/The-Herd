using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Gameplay.SanityEffects
{
    public class EnemyShadowsSpawner : MonoBehaviour
    {
        private SanityEffectsConfig config;
        private Transform playerTransform;
        private Coroutine spawnerCoroutine;
        private float currentChance = 5f;
        private float currentCooldown = 0f;


        public bool IsActive { get; private set; } = false;


        public void Initialize(SanityEffectsConfig config, Transform playerTransform)
        {
            this.config = config;
            this.playerTransform = playerTransform;
        }


        public void StartSpawner()
        {
            spawnerCoroutine = StartCoroutine(SpawnerRoutine());
            IsActive = true;
        }


        private IEnumerator SpawnerRoutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(config.TimeBetweenSpawnerAttempts);

                if (currentCooldown <= 0)
                    TryToSpawnShadows();

                yield return null;
            }
        }


        private void TryToSpawnShadows()
        {
            float value = Random.Range(0f, 100f);
            if (value >= 100 - currentChance)
            {
                SpawnShadows();
                currentCooldown = config.AttemptsCooldown;
            }
        }


        private void SpawnShadows()
        {
            int enemiesAmount = Random.Range(config.MinEnemiesAmount, config.MaxEnemiesAmount + 1);

            for (int i = 0; i < enemiesAmount; i++)
            {
                GameObject enemyToSpawn = config.PossibleEnemiesToSpawn[Random.Range(0, config.PossibleEnemiesToSpawn.Count)];

                float distanceToSpawn = Random.Range(config.MinSpawnDistance, config.MaxSpawnDistance);

                float angle = Random.Range(0f, 360f);

                Vector3 spawnPosition = playerTransform.position + (Quaternion.Euler(0, angle, 0f) * playerTransform.forward * distanceToSpawn);

                if (NavMesh.SamplePosition(spawnPosition, out NavMeshHit hit, 2, NavMesh.AllAreas))
                {
                    spawnPosition = hit.position;

                    Quaternion rotation = Quaternion.LookRotation(playerTransform.position - spawnPosition, Vector3.up);

                    GameObject newEnemy = Instantiate(enemyToSpawn, spawnPosition, rotation);

                    float lifeTime = Random.Range(config.MinEnemyDuration, config.MaxEnemyDuration);

                    StartCoroutine(RemoveEnemy(newEnemy, lifeTime));
                }

                Debug.Log($"Spawned {enemiesAmount}");
            }
        }


        public void StopSpawner()
        {
            if (spawnerCoroutine != null)
            {
                StopCoroutine(spawnerCoroutine);
                spawnerCoroutine = null;
            }

            IsActive = false;
        }


        private void Update()
        {
            if (currentCooldown > 0)
                currentCooldown -= Time.deltaTime;

            if (currentCooldown < 0f)
                currentCooldown = 0f;
        }


        public void UpdateCurrentChance(float sanityPercentage)
        {
            currentChance = Mathf.Lerp(20f, 5f, Mathf.InverseLerp(40f, 75f, sanityPercentage));
        }


        private IEnumerator RemoveEnemy(GameObject enemyToDestroy, float destroyTime)
        {
            yield return new WaitForSeconds(destroyTime);

            Destroy(enemyToDestroy.gameObject);

            yield return null;
        }
    }
}
