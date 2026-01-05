using System.Collections;
using UnityEngine;

public class SoftSceneCaller : MonoBehaviour
{
    [SerializeField] private string[] text;
    [SerializeField] private int time;
    [SerializeField] private GameObject speaker;
    [SerializeField] private bool called = false;
    //[SerializeField] private bool freeze = false;

    public void OneTimeCall()
    {
        if (!called)
        {
            CutsceneManager.SpawnSoftScene(text, time, speaker);
            called = true;
            /*if (freeze)
            {

            }     */
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

    /*
     * can be uncommented after player merge.
     * IEnumerator FreezeUntilDone()
    {

        yield return new WaitForSeconds(time * text.Length);
        PlayerInputHandler.EnableAllPlayerActions();
    }     */
}
