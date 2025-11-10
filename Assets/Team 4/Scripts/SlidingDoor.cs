using UnityEngine;
using System.Collections;

public class SlidingDoor : MonoBehaviour
{
    [SerializeField] private float slideHeight = 5f;
    [SerializeField] private float slideSpeed = 2f;
    private Light doorLight;

    private Vector3 startPosition;
    private bool isSliding = false;
    private bool isLocked = true;
    private bool isOpen = false;

    private void Awake()
    {
        doorLight = GetComponentInChildren<Light>();

        TurnLightOff();
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger entered by: " + other.name);
        if (other.CompareTag("Player"))
        {
            if (!isLocked && !isOpen)
            {
                Open();
                TurnLightOff();
            }
        }
    }

    public void Unlock()
    {
        isLocked = false;
        TurnLightOn();
    }

    public void Lock()
    {
        isLocked = true;
        TurnLightOff();
    }

    public void Open()
    {
        if (isOpen) return;
        if (isSliding) return;

        startPosition = transform.position;

        StartCoroutine(SlideDoor());

        isOpen = true;
    }

    private IEnumerator SlideDoor()
    {
        isSliding = true;
        Vector3 targetPosition = startPosition - new Vector3(0, slideHeight, 0);
        
        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, slideSpeed * Time.deltaTime);
            yield return null;
        }
        
        transform.position = targetPosition;
        isSliding = false;
    }

    public void Close()
    {
        if (isSliding) return;
        
        StartCoroutine(RaiseDoor());
    }

    private IEnumerator RaiseDoor()
    {
        isSliding = true;
        
        while (Vector3.Distance(transform.position, startPosition) > 0.01f)
        {
            transform.position = Vector3.Lerp(transform.position, startPosition, slideSpeed * Time.deltaTime);
            yield return null;
        }
        
        transform.position = startPosition;
        isSliding = false;
    }

    public void TurnLightOn()
    {
        if (doorLight != null)
        {
            doorLight.enabled = true;
        }
    }

    public void TurnLightOff()
    {
        if (doorLight != null)
        {
            doorLight.enabled = false;
        }
    }
}
