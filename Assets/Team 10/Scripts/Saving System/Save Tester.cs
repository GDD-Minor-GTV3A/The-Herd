using UnityEngine;

public class SaveTester : MonoBehaviour
{
    private string path;

    private void Start()
    {
        path = Application.persistentDataPath + "/testsave.json";
        Debug.Log("Save path: " + path);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            SaveSystem.SaveGame(path);
        }

        if (Input.GetKeyDown(KeyCode.F9))
        {
            SaveSystem.LoadGame(path);
        }
    }
}
