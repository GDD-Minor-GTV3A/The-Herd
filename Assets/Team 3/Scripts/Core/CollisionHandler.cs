using UnityEditor;

using UnityEngine;

public class CollisionHandler : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Debug.Log("Player entered the cube");
        }
        else
        {
            Debug.Log(other);
        }

    }
}