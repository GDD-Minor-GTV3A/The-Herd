using UnityEngine;
using UnityEngine.Events;

public class SpawnTrigger : MonoBehaviour
{
    //private bool triggered = false;

    public UnityEvent onTriggered;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
       // if (triggered) return;
        //triggered = true;
        onTriggered.Invoke();
    }
}
