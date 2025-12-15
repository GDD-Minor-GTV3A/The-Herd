using Gameplay.CameraSettings;
using Gameplay.Player;
using UnityEngine;

public class EffectsTest : MonoBehaviour
{
    public CameraManager CameraManager;

    private bool isActive = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            isActive = !isActive;
            CameraManager.SetContinuousCameraShakes(isActive, 2.0f);
        }
    }
}
