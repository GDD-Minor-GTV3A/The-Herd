using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    public Transform monsert, player;

    void Update()
    {
        if (player != null)
        {
            monsert.transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
        }
    }
}
