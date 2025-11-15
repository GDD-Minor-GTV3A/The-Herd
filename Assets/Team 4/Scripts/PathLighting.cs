using UnityEngine;

public class PathLighting : MonoBehaviour
{
    [SerializeField] private PathLighting previousLight;
    [SerializeField] private PathLighting nextLight;

    private Light currentLight;

    private void Awake()
    {
        currentLight = GetComponentInChildren<Light>();
        
        if (previousLight == null)
        {
            currentLight.enabled = true;
        }
        else
        {
            currentLight.enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (nextLight != null && currentLight.enabled)
            {
                nextLight.TurnOn();
            }
            
            if (previousLight != null)
            {
                previousLight.TurnOff();
            }
        }
    }

    public void TurnOn()
    {
        if (currentLight != null)
        {
            currentLight.enabled = true;
        }
    }

    public void TurnOff()
    {
        if (currentLight != null)
        {
            currentLight.enabled = false;
        }
    }
}
