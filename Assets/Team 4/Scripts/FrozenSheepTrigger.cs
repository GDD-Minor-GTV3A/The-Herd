using UnityEngine;

public class FrozenSheepTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Spawn sheep at same position
            LevelManagerLevel2.Instance.SpawnSheep(transform.position);

            Destroy(gameObject);
        }
    }
}
