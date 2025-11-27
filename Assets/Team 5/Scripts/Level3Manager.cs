using UnityEngine;

public class Level3Manager : MonoBehaviour
{
    [SerializeField] GameObject navMesh;

    void Awake()
    {
        navMesh.SetActive(true);
    }

    void Update()
    {
        
    }
}
