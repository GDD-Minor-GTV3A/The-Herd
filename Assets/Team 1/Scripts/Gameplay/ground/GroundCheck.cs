using PlasticGui.Configuration.CloudEdition.Welcome;

using Unity.VisualScripting.Antlr3.Runtime.Tree;

using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    public float checkDistance = 1.2f;     // How far down to raycast
    public LayerMask groundMask;          // Which layers count as ground
    private int count = 0;
    private GameObject surface = null;
    private Material mat = null;
    void Update()
    {
        count += 1;
        if (count == 60) {
            count = 0;
            RaycastHit hit;

            // Cast a ray downward from the player's position
            if (Physics.Raycast(transform.position, Vector3.down, out hit, checkDistance, groundMask))
            {
                // What object did we hit?
                surface = hit.collider.gameObject;

                // Print the object name
                Debug.Log("Walking on: " + surface.name);

                // If you want the material:
                var renderer = surface.GetComponent<Renderer>();
                if (renderer != null)
                {
                    mat = renderer.sharedMaterial;
                    Debug.Log("Surface material: " + mat.name);
                }

                // If you want the tag:
                Debug.Log("Surface tag: " + surface.tag);
            }
            else
            {
                Debug.Log("Not standing on anything");
            }
        }
    }
    public Material GetCurrentSurfaceMaterial()
    {
        return mat;
    }
    public GameObject GetCurrentSurface()
    {
        return surface;
    }
}
