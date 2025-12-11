using UnityEngine;

public class ContinueButton : MonoBehaviour
{
    [Tooltip("Reference to the PauseMenu component to resume the game")]
    public PauseMenu pauseMenu;

    // Called by the Continue button's OnClick
    public void OnContinue()
    {
        if (pauseMenu != null)
            pauseMenu.Resume();
    }
}
