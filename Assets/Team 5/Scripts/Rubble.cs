using UnityEngine;

public class Rubble : MonoBehaviour
{
    [SerializeField] Transform parentCheck;

    [SerializeField] GameObject boulder;

    void Update()
    {
        if (boulder.transform.localPosition.y < -4f)
        {
            boulder.transform.Translate(Vector3.up * 2f * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(parentCheck.childCount <= 2)
        {
            // boulder.SetActive(true);
            // Destroy(gameObject);
        }
        else if(other.transform.IsChildOf(parentCheck))
        {
            Destroy(other.gameObject);
        }
    }
}
