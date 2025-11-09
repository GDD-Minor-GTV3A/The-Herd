using UnityEngine;
using System.Collections;

public class OpenShop : MonoBehaviour
{
    [SerializeField] private GameObject shopUI;
    [SerializeField] private GameObject nPCInteract;

    private bool enteredShop = false;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.E) && enteredShop)
        {
            // Disable interaction button
            if (!shopUI.activeInHierarchy)
            {
                nPCInteract.SetActive(false);
            }
            else
            {
                nPCInteract.SetActive(true);
            }

            // Show shop UI
            shopUI.SetActive(!shopUI.activeInHierarchy);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            enteredShop = true;
            nPCInteract.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            enteredShop = false;
            nPCInteract.SetActive(false);
        }
    }
}
