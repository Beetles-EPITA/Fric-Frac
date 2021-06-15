using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace Menus
{
    public class SettingsMenu : MonoBehaviour
    {

        [SerializeField] private VerticalLayoutGroup volumeLayout;
        [SerializeField] private GameObject prefabVolume;
        [SerializeField] private AudioMixer audioMixer;
        [SerializeField] private Image muteImageButton;

        [SerializeField] private Sprite volumeOnSprite;
        [SerializeField] private Sprite volumeOffSprite;
        
        
        private static AudioMixer _audioMixerStatic;
        
        public TMP_Dropdown resolutionDropdown;
        public TMP_Dropdown qualityDropdown;
        public Toggle fullScreenToggle;

        Resolution[] _resolutions;

        public static SettingsMenu Instance;

        private void Awake()
        {
            _resolutions = Screen.resolutions;
            _audioMixerStatic = audioMixer;
            Instance = this;
        }

        void Start()
        {
            SetupDropdownResolution();
            
            InitResolutionAndScreenMode();
            InitQuality();
            InitVolume();
            InitMute();


        }

        private void InitVolume()
        {
            foreach (var mixer in audioMixer.FindMatchingGroups(""))
            {
                string sourcePath = "volume." + mixer.name.ToLower();
                float volume = PlayerPrefs.HasKey(sourcePath) ? PlayerPrefs.GetFloat(sourcePath) : -10f;
                mixer.audioMixer.SetFloat(sourcePath, volume);
                GameObject volumeObject = Instantiate(prefabVolume, volumeLayout.transform);
                volumeObject.GetComponent<VolumeSlider>().path = sourcePath;
                volumeObject.GetComponentInChildren<Text>().text = mixer.name;
                volumeObject.GetComponentInChildren<Slider>().value = volume;
            }
        }

        private void InitMute()
        {
            string sourcePath = "volume.master";
            float volume = PlayerPrefs.HasKey(sourcePath) ? PlayerPrefs.GetFloat(sourcePath) : -10f;
            if (volume <= -80f)
                muteImageButton.sprite = volumeOffSprite;
        }

        private static void ToggleMute()
        {
            string sourcePath = "volume.master";
            float volume = PlayerPrefs.HasKey(sourcePath) ? PlayerPrefs.GetFloat(sourcePath) : -10f;
            if (volume <= -80f)
            {
                volume = -10f;
                Instance.muteImageButton.sprite = Instance.volumeOnSprite;
            }
            else
            {
                volume = -80f;
                Instance.muteImageButton.sprite = Instance.volumeOffSprite;
            }
            _audioMixerStatic.SetFloat(sourcePath, volume);
            PlayerPrefs.SetFloat(sourcePath, volume);
        }

        public static void SetVolume(float volume, string path)
        {
            if (volume <= -40f)
            {
                volume = -80f;
            }
            _audioMixerStatic.SetFloat(path, volume);
            PlayerPrefs.SetFloat(path, volume);
            Instance.InitMute();
        }

        private void InitResolutionAndScreenMode()
        {
            Screen.fullScreen = !PlayerPrefs.HasKey("fullscreen") || PlayerPrefs.GetInt("fullscreen") != 0;
            Resolution resolution = PlayerPrefs.HasKey("resolution") ? _resolutions[PlayerPrefs.GetInt("resolution")] : Screen.currentResolution;
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
            fullScreenToggle.isOn = Screen.fullScreen;
        }

        public void SetResolution(int resolutionIndex)
        {
            Resolution resolution = _resolutions[resolutionIndex];
            PlayerPrefs.SetInt("resolution", resolutionIndex);
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        }

        public void SetFullscreen (bool isFullscreen)
        {
            PlayerPrefs.SetInt("fullscreen", isFullscreen ? 1 : 0);
            Screen.fullScreen = isFullscreen;
        }

        private void InitQuality()
        {
            int quality = PlayerPrefs.HasKey("quality") ? PlayerPrefs.GetInt("quality") : QualitySettings.GetQualityLevel();
            QualitySettings.SetQualityLevel(quality);
            qualityDropdown.value = quality;
        }

        public void SetQuality (int qualityIndex)
        {
            PlayerPrefs.SetInt("quality", qualityIndex);
            QualitySettings.SetQualityLevel(qualityIndex);
        }
    
        private void SetupDropdownResolution()
        {
            resolutionDropdown.ClearOptions();
        
            List<string> options = new List<string>();

            int currentResolutionIndex = 0;
            for (int i = 0; i < _resolutions.Length; i++)
            {
                string option = _resolutions[i].width + " x " + _resolutions[i].height + " " + _resolutions[i].refreshRate + "fps";
                options.Add(option);

                if (_resolutions[i].width == Screen.currentResolution.width && _resolutions[i].height == Screen.currentResolution.height)
                {
                    currentResolutionIndex = i;
                }
            }
        
            resolutionDropdown.AddOptions(options);
            resolutionDropdown.value = PlayerPrefs.HasKey("resolution") ? PlayerPrefs.GetInt("resolution") : currentResolutionIndex;
            resolutionDropdown.RefreshShownValue();
        }

    }
}
