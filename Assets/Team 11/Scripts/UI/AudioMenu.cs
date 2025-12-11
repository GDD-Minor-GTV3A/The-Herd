using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI
{
    public class AudioMenu : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private Slider masterSlider;
        [SerializeField] private Slider musicSlider;
        [SerializeField] private Slider sfxSlider;

        public UnityAction<float, VolumeType> OnVolumeChanged;

        void OnEnable()
        {
            masterSlider.onValueChanged.AddListener(ChangeMasterVolume);
            musicSlider.onValueChanged.AddListener(ChangeMusicVolume);
            sfxSlider.onValueChanged.AddListener(ChangeSFXVolume);
        }

        void OnDisable()
        {
            masterSlider.onValueChanged.RemoveListener(ChangeMasterVolume);
            musicSlider.onValueChanged.RemoveListener(ChangeMusicVolume);
            sfxSlider.onValueChanged.RemoveListener(ChangeSFXVolume);
        }

        private void ChangeMasterVolume(float volume)
        {
            OnVolumeChanged?.Invoke(volume, VolumeType.Master);
        }

        private void ChangeMusicVolume(float volume)
        {
            OnVolumeChanged?.Invoke(volume, VolumeType.Music);
        }

        private void ChangeSFXVolume(float volume)
        {
            OnVolumeChanged?.Invoke(volume, VolumeType.SFX);
        }
    }
}
