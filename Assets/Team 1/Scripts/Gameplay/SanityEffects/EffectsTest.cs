using Gameplay.Player;
using UnityEngine;

public class EffectsTest : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            PlayerInputHandler.SwitchControlMap();
        }
    }
}
