using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace Menus
{
    public class SettingsMenu : MonoBehaviour
    {

        public AudioMixer audiomixer;
    
        public Slider volumeSlider;
        public TMP_Dropdown resolutionDropdown;
        public TMP_Dropdown qualityDropdown;
        public Toggle fullScreenToggle;

        Resolution[] _resolutions;

    


        private void Awake()
        {
            _resolutions = Screen.resolutions;
        }

        void Start()
        {
            SetupDropdownResolution();
            
            InitResolutionAndScreenMode();
            InitQuality();
            InitVolume();

            
        }

        private void InitVolume()
        {
            float volume = PlayerPrefs.HasKey("volume.master") ? PlayerPrefs.GetFloat("volume.master") : 0f;
            audiomixer.SetFloat("volume", volume);
            volumeSlider.value = volume;
        }
    
        public void SetVolume(float volume)
        {
            PlayerPrefs.SetFloat("volume.master", volume);
            audiomixer.SetFloat("volume", volume);
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
                string option = _resolutions[i].width + " x " + _resolutions[i].height;
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
