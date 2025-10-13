using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 50f;
    [SerializeField] private float lifeTime = 5f;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Shoot(Vector3 direction)
    {
        rb.linearVelocity = direction * speed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"Bullet hit: {collision.gameObject.name}");
        // Optionally apply damage:
        // var health = collision.gameObject.GetComponent<Health>();
        // if(health != null) health.TakeDamage(damage);
    }
}