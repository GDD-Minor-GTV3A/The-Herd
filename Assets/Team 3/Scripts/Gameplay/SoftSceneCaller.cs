using UnityEngine;

public class SoftSceneCaller : MonoBehaviour
{
    [SerializeField] private string[] text;
    [SerializeField] private int time;
    [SerializeField] private GameObject speaker;
    [SerializeField] private bool called = false;

    public void OneTimeCall()
    {
        if (!called)
        {
            CutsceneManager.SpawnSoftScene(text, time, speaker);
            called = true;
        }
    }

    public void OneTimePlayer()
    {
        if (!called)
        {
            CutsceneManager.SpawnSoftScene(text, time, speaker);
            called = true;
        }
    }
}
