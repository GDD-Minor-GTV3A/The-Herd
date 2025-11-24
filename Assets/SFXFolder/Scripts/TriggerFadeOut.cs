using UnityEngine;

public class TriggerFadeOut : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        AudioManager.Instance.PlayFadeOut();
        Destroy(gameObject);
    }
}
