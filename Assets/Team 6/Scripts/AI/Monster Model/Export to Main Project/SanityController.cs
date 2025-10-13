using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SanityController : MonoBehaviour
{
    public int minSheepToAffect = 2;
    public int maxSheepToAffect = 3;
    public int sanityGainPerTick = 1;
    public float tickInterval = 1f;

    private DetectSheep detector;

    private void Start()
    {
        detector = GetComponent<DetectSheep>();
        if (detector == null)
        {
            Debug.LogError("SanityController requires DetectSheep component.");
            enabled = false;
            return;
        }

        StartCoroutine(SanityGainRoutine());
    }

    private IEnumerator SanityGainRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(tickInterval);

        while (true)
        {
            yield return wait;

            List<Transform> sheepInRange = new List<Transform>(detector.visibleTargets);
            int sheepToAffect = Random.Range(minSheepToAffect, maxSheepToAffect + 1);

            if (sheepInRange.Count == 0) continue;

            if (sheepInRange.Count <= sheepToAffect)
            {
                foreach (Transform sheep in sheepInRange)
                    TryAddSanity(sheep);
            }
            else
            {
                for (int i = 0; i < sheepToAffect; i++)
                {
                    int index = Random.Range(0, sheepInRange.Count);
                    Transform chosenSheep = sheepInRange[index];
                    TryAddSanity(chosenSheep);
                    sheepInRange.RemoveAt(index); // Avoid duplicates this tick
                }
            }
        }
    }

    private void TryAddSanity(Transform sheep)
    {
        if (sheep.TryGetComponent<SheepSanity>(out var sanity))
        {
            sanity.GainSanity(sanityGainPerTick);
        }
    }
}
