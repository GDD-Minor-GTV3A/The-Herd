using UnityEngine;

public class QuitButton : MonoBehaviour
{
    // Called by the Quit button's OnClick
    public void OnQuitApplication()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
