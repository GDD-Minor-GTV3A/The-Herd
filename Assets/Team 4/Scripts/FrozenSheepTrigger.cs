using UnityEngine;

/// <summary>
/// When sheep escape, the sheep will go to these points. When the player collides with the trigger, the sheep will be teleported to the trigger's position.
/// </summary>
public class FrozenSheepTrigger : MonoBehaviour
{
    [SerializeField] private LevelManagerLevel2 LevelManager;
    public bool ContainsSheep = false;
    public GameObject Sheep { get; private set; }

    private void Start()
    {
        // Make itself and all child objects invisible at start
        SetVisibility(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (ContainsSheep && Sheep != null)
            {
                Sheep.transform.position = transform.position;
                SetVisibility(false);
                LevelManager.SheepCollected();
            }
        }
    }

    public void SetSheep(GameObject sheep)
    {
        Sheep = sheep;
        ContainsSheep = true;
        SetVisibility(true);
    }

    private void SetVisibility(bool isVisible)
    {
        // Set visibility for this object and all child renderers
        Renderer[] allRenderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in allRenderers)
        {
            renderer.enabled = isVisible;
        }
    }
}
