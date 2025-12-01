using UnityEditor;

using UnityEngine;
using UnityEngine.Events;

public class CollisionOffshoot : MonoBehaviour
{
    /// <summary>
    /// The event that plays when the player enters this collider
    /// </summary>
    public UnityEvent collisionResult;

    /// <summary>
    /// whether the eventcall disables itself after usage, making it callable only once
    /// </summary>
    [SerializeField]
    private bool oneTimeEvent = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            collisionResult?.Invoke();
            if (oneTimeEvent)
            {
                transform.gameObject.SetActive(false);
            }
        }
        else
        {
            Debug.Log(other);
        }

    }
}
