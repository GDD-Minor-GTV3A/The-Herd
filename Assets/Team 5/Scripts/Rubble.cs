using UnityEngine;

public class Rubble : MonoBehaviour
{
    [SerializeField] Transform parentCheck;

    [SerializeField] GameObject boulder;

    /// <summary>
    /// Once the cave-in gets enabled, the boulder will rise to a certain number.
    /// </summary>
    void Update()
    {
        if (boulder.transform.localPosition.y < -4f)
        {
            boulder.transform.Translate(Vector3.up * 2f * Time.deltaTime);
        }
    }

    /// <summary>
    /// Method used to remove the pebbles that fall down for the animation.
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if(parentCheck.childCount <= 2)
        {
            // boulder.SetActive(true);
            // Destroy(gameObject);
        }
        else if(other.transform.IsChildOf(parentCheck))
        {
            Destroy(other.gameObject);
        }
    }
}
