using UnityEngine;

public class RayProjectile : MonoBehaviour
{
    public float speed = 5f;   // slowed down for visibility
    public float lifetime = 4f;

    private float timer = 0f;

    void Update()
    {
        // Move slowly forward so it's visible
        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        timer += Time.deltaTime;
        if (timer >= lifetime)
            Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("[RayProjectile] Player hit!");
           
        }
    }
}
