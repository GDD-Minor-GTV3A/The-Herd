using UnityEngine;

public class TriggerFadeOut : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (CompareTag("Player"))
        {
            AudioManager.Instance.PlayFadeOut();
        }

        Destroy(gameObject);
    }
}
