using UnityEngine;

namespace UI
{
    public class OpenSettingsButton : MonoBehaviour
    {
        [Tooltip("Reference to the SettingsMenu component")]
        public SettingsMenu settingsMenu;

        // Called by the Settings button's OnClick
        public void OnOpenSettings()
        {
            if (settingsMenu != null)
                settingsMenu.OpenSettings();
        }
    }
}
