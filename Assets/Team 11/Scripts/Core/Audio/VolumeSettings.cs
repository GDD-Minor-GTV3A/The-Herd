using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace Core.Audio
{
    public class VolumeSettings : MonoBehaviour
    {
        [Header("Mixer")]
        [SerializeField] private AudioMixer audioMixer;

        [Header("Sliders")]
        [SerializeField] private Slider masterSlider;
        [SerializeField] private Slider ambienceSlider;
        [SerializeField] private Slider musicSlider;
        [SerializeField] private Slider sfxSlider;
        [SerializeField] private Slider uiSlider;

        private const string MASTER_VOLUME = "MasterVolume";
        private const string AMBIENCE_VOLUME = "AmbienceVolume";
        private const string MUSIC_VOLUME = "MusicVolume";
        private const string SFX_VOLUME = "SoundFXVolume";
        private const string UI_VOLUME = "UIVolume";

        void Awake()
        {
            masterSlider.onValueChanged.AddListener(SetMasterVolume);
            ambienceSlider.onValueChanged.AddListener(SetAmbienceVolume);
            musicSlider.onValueChanged.AddListener(SetMusicVolume);
            sfxSlider.onValueChanged.AddListener(SetSFXVolume);
            uiSlider.onValueChanged.AddListener(SetUIVolume);
        }

        void Start() =>
            Load();

        void OnDestroy() =>
            Save();

        private void SetMasterVolume(float value) =>
            SetVolume(MASTER_VOLUME, value);

        private void SetAmbienceVolume(float value) =>
            SetVolume(AMBIENCE_VOLUME, value);

        private void SetMusicVolume(float value) =>
            SetVolume(MUSIC_VOLUME, value);

        private void SetSFXVolume(float value) =>
            SetVolume(SFX_VOLUME, value);

        private void SetUIVolume(float value) =>
            SetVolume(UI_VOLUME, value);

        private void SetVolume(string key, float value) =>
            audioMixer.SetFloat(key, Mathf.Log10(value) * 20f);

        private void Save()
        {
            PlayerPrefs.SetFloat(MASTER_VOLUME, masterSlider.value);
            PlayerPrefs.SetFloat(AMBIENCE_VOLUME, ambienceSlider.value);
            PlayerPrefs.SetFloat(MUSIC_VOLUME, musicSlider.value);
            PlayerPrefs.SetFloat(SFX_VOLUME, sfxSlider.value);
            PlayerPrefs.SetFloat(UI_VOLUME, uiSlider.value);
        }

        private void Load()
        {
            var _masterVolume = PlayerPrefs.GetFloat(MASTER_VOLUME, 1f);
            var _ambienceVolume = PlayerPrefs.GetFloat(AMBIENCE_VOLUME, 1f);
            var _musicVolume = PlayerPrefs.GetFloat(MUSIC_VOLUME, 1f);
            var _sfxVolume = PlayerPrefs.GetFloat(SFX_VOLUME, 1f);
            var _uiVolume = PlayerPrefs.GetFloat(UI_VOLUME, 1f);

            masterSlider.value = _masterVolume;
            ambienceSlider.value = _ambienceVolume;
            musicSlider.value = _musicVolume;
            sfxSlider.value = _sfxVolume;
            uiSlider.value = _uiVolume;
            SetMasterVolume(_masterVolume);
            SetAmbienceVolume(_ambienceVolume);
            SetMusicVolume(_musicVolume);
            SetSFXVolume(_sfxVolume);
            SetUIVolume(_uiVolume);
        }
    }
}
