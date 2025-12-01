using Gameplay.CameraSettings;

using UnityEngine;

public class TriggerAnimationFromOutside : MonoBehaviour
{
   // public Animation animation;
    public Animator animator;           // drag the object's Animator here
    [SerializeField] private CameraManager cam;
    public string triggerName = "PlayAnim"; // your trigger name

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {

            Debug.Log("KUR");
            animator.SetBool("Open", true);
            cam.ShakeCamera(1);
            Debug.Log("Shake");
        }
    }
}
