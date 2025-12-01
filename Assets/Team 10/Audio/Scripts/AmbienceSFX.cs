using UnityEngine;

public class AmbienceSFX : MonoBehaviour
{
    public AudioSource Sample1;

    public float minDelay = 6f;
    public float maxDelay = 10f;

    public float reactionMin = 6f;
    public float reactionMax = 10f;

    public float loopMin = 6f;
    public float loopMax = 10f;

    void Start()
    {
        StartCoroutine(PlaySequence());
    }

    private System.Collections.IEnumerator PlaySequence()
    {
        while (true)
        {
            float initialWait = Random.Range(minDelay, maxDelay);
            yield return new WaitForSeconds(initialWait);

            int loopCount = Random.Range(3, 7); 
            float pitch = Random.Range(0.85f, 1.15f);
            float pan = Random.Range(-1f, 1f);

            Sample1.pitch = pitch;
            Sample1.panStereo = pan;

            for (int i = 0; i < loopCount; i++)
            {
                Sample1.Play();
                yield return new WaitForSeconds(0.5f);
            }

            float reactionWait = Random.Range(reactionMin, reactionMax);
            yield return new WaitForSeconds(reactionWait);

            loopCount = Random.Range(2, 6);
            pitch = Random.Range(0.85f, 1.15f);
            pan = Random.Range(-1f, 1f);

            Sample1.pitch = pitch;
            Sample1.panStereo = pan;

            for (int i = 0; i < loopCount; i++)
            {
                Sample1.Play();
                yield return new WaitForSeconds(0.5f);
            }

            float loopWait = Random.Range(loopMin, loopMax);
            yield return new WaitForSeconds(loopWait);
        }
    }
}
