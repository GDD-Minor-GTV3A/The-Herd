using UnityEngine;
using UnityEngine.Audio;
using Core.Shared.Utilities;

namespace UI
{
    public class SettingsMenu : MonoBehaviour
    {
        [Header("Mixer")]
        [Required][SerializeField] private AudioMixer mixer;

        [Header("Main Settings Menu")]
        [SerializeField] private GameObject settingsMenu;

        [Header("Menus")]
        [SerializeField] private GameObject graphicsMenu;
        [SerializeField] private GameObject audioMenu;
        [SerializeField] private GameObject controlsMenu;

        private AudioMenu audioSetting;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            audioSetting = audioMenu.GetComponent<AudioMenu>();
            audioSetting.OnVolumeChanged += SetVolume;

            OpenSettings();
        }

        public void OpenSettings()
        {
            settingsMenu.SetActive(true);

            // Disable all menus
            graphicsMenu.SetActive(false);
            audioMenu.SetActive(false);
            controlsMenu.SetActive(false);
        }

        private void SetVolume(float value, VolumeType type)
        {
            float volume = Mathf.Log10(value) * 20f;

            switch (type)
            {
                case VolumeType.Master:
                    mixer.SetFloat("MasterVolume", volume);
                break;
                case VolumeType.Music:
                    mixer.SetFloat("MusicVolume", volume);
                break;
                case VolumeType.SFX:
                    mixer.SetFloat("SoundFXVolume", volume);
                break;
            }
        }

        public void OpenGraphicsMenu()
        {
            settingsMenu.SetActive(false);
            graphicsMenu.SetActive(true);
        }
        
        public void OpenAudioMenu()
        {
            settingsMenu.SetActive(false);
            audioMenu.SetActive(true);
        }

        public void OpenControlsMenu()
        {
            settingsMenu.SetActive(false);
            controlsMenu.SetActive(true);
        }
    }

    public enum VolumeType
    {
        Master,
        Music,
        SFX
    }
}
