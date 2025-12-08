using UnityEngine;

public class CutsceneManager : MonoBehaviour
{
    [SerializeField] private GameObject softCutscenePrefab;
    //[SerializeField] private GameObject hardCutscenePrefab;
    public static CutsceneManager Instance;


    private void Awake()
    {
        Instance = this;

    }

    public static void SpawnSoftScene(string[] dialogue, int readTime, GameObject sceneParent)
    {
        GameObject newObject = Instantiate(Instance.softCutscenePrefab, sceneParent.transform.position, Quaternion.identity, sceneParent.transform);
        SoftCutscene settings = newObject.GetComponent<SoftCutscene>();
        settings.Initialize(dialogue, readTime);
    }


    /*public static GameObject StartHardCutscene()
    {
        GameObject activeCutscene = Instantiate(Instance.hardCutscenePrefab);
        return activeCutscene;
    }    */
}
