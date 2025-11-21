using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Player_Controller : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 8f;
    public float rotationSpeed = 10f;

    [Header("Footsteps")]
    public int footstepSFXIndex = 0;      // Index in AudioManager.sfxClips
    public float footstepInterval = 0.5f; // seconds between footsteps
    [Range(0f, 1f)] public float footstepVolume = 0.5f; // Footstep-specific volume

    [Header("References")]
    public Camera mainCamera;

    private CharacterController controller;
    private float footstepTimer = 0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");
        Vector3 inputDir = new Vector3(moveX, 0f, moveZ).normalized;

        if (inputDir.magnitude >= 0.1f)
        {
            Vector3 cameraForward = mainCamera.transform.forward;
            Vector3 cameraRight = mainCamera.transform.right;
            cameraForward.y = 0f;
            cameraRight.y = 0f;
            cameraForward.Normalize();
            cameraRight.Normalize();

            Vector3 moveDir = cameraForward * inputDir.z + cameraRight * inputDir.x;

            float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed;

            controller.SimpleMove(moveDir * currentSpeed);

            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            // Footsteps
            footstepTimer -= Time.deltaTime;
            if (footstepTimer <= 0f && footstepSFXIndex >= 0)
            {
                AudioManager.Instance.PlaySFX(footstepSFXIndex, footstepVolume);
                footstepTimer = footstepInterval / (currentSpeed / walkSpeed);
            }
        }
        else
        {
            footstepTimer = 0f;
        }
    }
}