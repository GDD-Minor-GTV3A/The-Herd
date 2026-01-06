using UnityEngine;

public class DestroySpawnerIfLeftTooLong : MonoBehaviour
{
    [Header("References")]
    public ShamanSpawner spawner;   

    [Header("Settings")]
    public float leaveDelay = 5f;   
    private bool hasEnteredOnce = false;
    private float awayTimer = 0f;

    void Reset()
    {
        spawner = GetComponent<ShamanSpawner>();
    }

    void Update()
    {
        if (spawner == null) return;
        
        if (spawner.Triggered)
        {
            hasEnteredOnce = true;
            awayTimer = 0f; 
            return;
        }
        
        if (!hasEnteredOnce) return;
        
        awayTimer += Time.deltaTime;

        if (awayTimer >= leaveDelay)
        {
            Destroy(gameObject);
        }
    }
}