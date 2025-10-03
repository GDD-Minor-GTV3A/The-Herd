using UnityEngine;

public class Destroy : MonoBehaviour
{
    public GameObject Destroyed;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "hit")
        {
            Instantiate(Destroyed, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }
}
