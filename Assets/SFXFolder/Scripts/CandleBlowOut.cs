using UnityEngine;

public class CandleBlowOut : MonoBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        AudioManager.Instance.PlayCandleBlowOut();
        Destroy(gameObject);
    }
}
