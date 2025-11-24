using UnityEngine;

public class AmbienceSFX : MonoBehaviour
{
    public AudioSource Sample1;
    public AudioSource Sample2;

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

            Sample1.pitch = Random.Range(0.85f, 1.15f);
            Sample1.panStereo = Random.Range(-1f, 1f);
            Sample1.Play();

            float reactionWait = Random.Range(reactionMin, reactionMax);
            yield return new WaitForSeconds(reactionWait);

            Sample2.pitch = Random.Range(0.85f, 1.15f);
            Sample2.panStereo = Random.Range(-1f, 1f);
            Sample2.Play();

            float loopWait = Random.Range(loopMin, loopMax);
            yield return new WaitForSeconds(loopWait);
        }
    }
}
