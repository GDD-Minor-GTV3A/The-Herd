using UnityEngine;

public class EnemySpawnpoint : MonoBehaviour
{
    private enum enSpawnMethod {SpawnRandom, SpawnAll, SpawnFirst}
    [SerializeField] private enSpawnMethod spawnMethod;
    [SerializeField] private GameObject[] enemies;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public Transform enemyParent;

    // private void Start()
    // {
    //     enemyParent = GameObject.Find("EnemyManager").transform;
    // }

    /// <summary>
    /// Spawn function, spawns the enemies that it as been assigned. Can spawn in 3 different methods 
    /// </summary>
    /// <param name="other"></param>
    public void spawn()
    {
        Debug.Log("Spawn");
        switch (spawnMethod) {
            case enSpawnMethod.SpawnAll:
                foreach (GameObject _enemy in enemies) {
                    Instantiate(_enemy, transform.position, Quaternion.identity, enemyParent);
                }
                break;
            case enSpawnMethod.SpawnFirst:
                Instantiate(enemies[0], transform.position, Quaternion.identity, enemyParent);
                break;
            case enSpawnMethod.SpawnRandom:
                GameObject enemy = enemies[Random.Range(0, enemies.Length - 1)];
                Instantiate(enemy, transform.position, Quaternion.identity, enemyParent);
                break;
        }
    }
}
