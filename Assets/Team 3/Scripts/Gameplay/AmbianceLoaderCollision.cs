using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class AmbianceLoaderCollision : MonoBehaviour
{
    /// <summary>
    /// The event that plays when the player enters this collider
    /// </summary>
    public UnityEvent<AudioClip[]> collisionResult;

    [Tooltip("Array of Ambiance sounds to pass along to the ambiance player.")]
    [SerializeField] private AudioClip[] areaAmbianceList;

    /// <summary>
    /// whether the eventcall disables itself after usage, making it callable only once
    /// </summary>
    [SerializeField]
    private bool oneTimeEvent = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            collisionResult?.Invoke(areaAmbianceList);
            if (oneTimeEvent)
            {
                transform.gameObject.SetActive(false);
            }
        }

    }
}

