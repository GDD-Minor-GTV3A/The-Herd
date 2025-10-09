using UnityEngine;

public class SpawnEnemyPoint : MonoBehaviour
{
    public GameObject Drekavac;

    public void SpawnEnemy()
    {
        Instantiate(Drekavac, transform.position, Quaternion.identity);
    }
}
