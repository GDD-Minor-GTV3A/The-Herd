using UnityEngine;

public class CaveInWalls : MonoBehaviour
{
    // [SerializeField] GameObject wallCaveIn;
    [SerializeField] GameObject[] objectsToEnable, objectsToDisable;

    /// <summary>
    /// Generic script for making walls cave in, or removing them.
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            foreach(GameObject enable in objectsToEnable)
            {
                enable.SetActive(true);
            }

            foreach(GameObject disable in objectsToDisable)
            {
                disable.SetActive(false);
            }
            
            Destroy(gameObject);
        }
    }
}
