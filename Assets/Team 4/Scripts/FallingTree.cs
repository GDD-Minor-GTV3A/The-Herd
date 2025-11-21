using UnityEngine;

public class FallingTree : MonoBehaviour
{
    [SerializeField] private GameObject treeChopped;
    private bool hasFallen = false;

    public void Execute()
    {
        if (hasFallen) return;

        hasFallen = true;

        Instantiate(treeChopped, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Execute();
        }
    }
}