using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    private int currentScene;
    private void Start()
    {
        currentScene = SceneManager.GetActiveScene().buildIndex;
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Backspace)) 
        {

            if(SceneManager.sceneCountInBuildSettings == currentScene+1)
                SceneManager.LoadScene(0);
            else
                SceneManager.LoadScene(currentScene + 1);

        }
    }
}
