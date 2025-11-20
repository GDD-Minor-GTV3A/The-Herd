using UnityEngine;

public class ReturnLight : MonoBehaviour
{
    [SerializeField] private PathLighting[] lights;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            foreach (PathLighting light in lights)
            {
                light.ForceOn();
            }
        }
    }
}
