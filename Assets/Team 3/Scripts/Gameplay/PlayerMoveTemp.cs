using UnityEngine;

public class PlayerMoveTemp : MonoBehaviour
{
    /// <summary>
    /// Player movement speed.
    /// </summary>
    [SerializeField]
    [Tooltip("Player movement speed.")]
    private float _speed = 5f;

    [SerializeField]
    [Tooltip("Player RigidBody.")]
    private Rigidbody _rigidBody; // Reference to Rigidbody component


    void Update()
    {
        // Get input for horizontal and vertical movement
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        // Calculate movement direction
        Vector3 move = transform.right * moveX + transform.forward * moveZ;

        // Apply movement
        _rigidBody.MovePosition(transform.position + move * _speed * Time.deltaTime);
    }
}
