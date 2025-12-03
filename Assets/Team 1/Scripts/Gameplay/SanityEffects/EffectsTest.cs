using Core.AI.Sheep;

using UnityEngine;

public class EffectsTest : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            SanityTracker.RemoveSanityPoints(16);
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            SanityTracker.AddSanityPoints(16);
        }
    }
}
