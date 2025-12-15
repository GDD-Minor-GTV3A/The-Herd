using UnityEngine;

public class Rubble : MonoBehaviour
{
    [SerializeField] Transform parentCheck;

    [SerializeField] GameObject boulder;

    private void OnTriggerEnter(Collider other)
    {
        if(parentCheck.childCount <= 2)
        {
            boulder.SetActive(true);
            Destroy(gameObject);
        }
        else if(other.transform.IsChildOf(parentCheck))
        {
            Destroy(other.gameObject);
        }
    }
}
