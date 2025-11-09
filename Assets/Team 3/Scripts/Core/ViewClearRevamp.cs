using UnityEngine;

public class ViewClearRevamp : MonoBehaviour
{
    [Tooltip("Selected layer by number for where objects get hidden if they hide the player")]
    [SerializeField] private int obstacleLayer;

    /// <summary>
    /// check on whether a layer has been chosen.
    /// </summary>
    private void Awake()
    {
        if (obstacleLayer == LayerMask.NameToLayer("Default"))
            Debug.Log("default layer detected on obstacleLayer, change to the obstaclelayer.");
    }

    /// <summary>
    /// logic call to make object transparent if it is currently hiding the player. only works on objects in the selected layer.
    /// </summary>
    /// <param name="other">object with a rigidbody that enters the trigger</param>
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == obstacleLayer)
        {
            other.gameObject.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
        }  
    }


    /// <summary>
    /// logic call to make object solid aggain after it exits the ViewClearTrigger. only works on objects in the selected layer.
    /// </summary>
    /// <param name="other">object with a rigidbody that exits the trigger</param>
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == obstacleLayer)
        {
            other.gameObject.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        }
    }


}
