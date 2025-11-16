using UnityEngine;

public class WallBreakTrigger : MonoBehaviour
{
    public bool forceApplied = false;
    public GameObject wallCollider1;
    public GameObject wallCollider2;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    { 
       
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            forceApplied = true;
            wallCollider1.SetActive(true);
            wallCollider2.SetActive(true);
        }
    }
}
