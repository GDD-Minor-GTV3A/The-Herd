using UnityEngine;

public class EnemySpawnpoint : MonoBehaviour
{
    private enum enSpawnMethod {SpawnRandom, SpawnAll, SpawnFirst}
    [SerializeField] private enSpawnMethod spawnMethod;
    [SerializeField] private GameObject[] enemies;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void spawn()
    {
        GameObject _enemy = enemies[Random.Range(0, enemies.Length - 1)];

        Instantiate(_enemy, transform.position, Quaternion.identity);
    }
}
