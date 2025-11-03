using UnityEngine;
using System.Collections;

public class SlidingDoor : MonoBehaviour
{
    [SerializeField] private float slideHeight = 5f;
    [SerializeField] private float slideSpeed = 2f;
    
    private Vector3 startPosition;
    private bool isSliding = false;

    public void Open()
    {
        if (isSliding) return;

        startPosition = transform.position;

        StartCoroutine(SlideDoor());
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
}
