using UnityEngine;

public class TriggerAnimationFromOutside : MonoBehaviour
{
   // public Animation animation;
    public Animator animator;           // drag the object's Animator here
    public string triggerName = "PlayAnim"; // your trigger name

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {

            Debug.Log("KUR");
            animator.SetBool("Open", true);
            
        }
    }
}
