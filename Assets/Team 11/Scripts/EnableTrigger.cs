using System.Linq;

using UnityEngine;

public class EnableTrigger : MonoBehaviour
{
    [SerializeField] private GameObject[] triggersToEnable;
    private bool activated = false;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            if (!activated) 
            {
                foreach (GameObject triggerToEnable in triggersToEnable) 
                { 
                    triggerToEnable.SetActive(true);
                    Debug.Log(triggerToEnable.ToString() + " was enabled");
                }
                activated = true;
            }
        }
    }
}
