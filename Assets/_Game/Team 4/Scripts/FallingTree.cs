using UnityEngine;

public class FallingTree : MonoBehaviour
{
    [SerializeField] private GameObject treeChopped;

    public void Execute()
    {
        Instantiate(treeChopped, transform.position, Quaternion.identity);
        Destroy(gameObject);

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Execute();
    }
}
