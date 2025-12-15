using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UI
{
    public class GraphicMenu : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private Slider brightnessSlider;
        [SerializeField] private TMP_Dropdown qualityDropdown;
        [SerializeField] private Toggle fullscreenDropdown;

        void OnEnable()
        {
            brightnessSlider.onValueChanged.AddListener(ChangeBrightness);
            qualityDropdown.onValueChanged.AddListener(ChangeQuality);
            fullscreenDropdown.onValueChanged.AddListener(ChangeFullscreen);
        }

        void OnDisable()
        {
            brightnessSlider.onValueChanged.RemoveListener(ChangeBrightness);
            qualityDropdown.onValueChanged.RemoveListener(ChangeQuality);
            fullscreenDropdown.onValueChanged.RemoveListener(ChangeFullscreen);
        }

        private void ChangeBrightness(float value)
        {
            // Need to change game brightness
        }

        private void ChangeQuality(int value)
        {
            QualitySettings.SetQualityLevel(value);
        }

        private void ChangeFullscreen(bool value)
        {
            Screen.fullScreen = value;
        }
    }
}
