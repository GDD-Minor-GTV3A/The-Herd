using UnityEngine;
using TMPro;

public class KnockOnDoorUI : MonoBehaviour
{
    [Header("References")]
    public KnockOnDoorController knockController;
    public TextMeshProUGUI knockText;

    void Start()
    {
        // Hide UI elements at start
        if (knockText != null)
            knockText.gameObject.SetActive(false);
    }

    void Update()
    {
        // Validate references
        if (knockController == null || knockText == null) return;

        // Show prompt if player is in range and not already knocking on door.
        if (knockController.PlayerInRange && !knockController.IsInteracting)
        {
            knockText.gameObject.SetActive(true);
            knockText.text = "Press E to knock on door";
        }
        // Show response if player is knocking on door.
        else if (knockController.IsInteracting)
        {
            knockText.gameObject.SetActive(true);
            knockText.text = "You knocked on the door!";
        }
        else
        {
            knockText.gameObject.SetActive(false);
        }
    }
}