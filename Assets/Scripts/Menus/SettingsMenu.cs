using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public void Volume (Slider slider)
    {
        AudioListener.volume = slider.value;
    }

    public AudioMixer audiomixer;

    public TMP_Dropdown resolutionDropdown;

    Resolution[] resolutions;

    void Start()
    {
        resolutions = Screen.resolutions;
        
        resolutionDropdown.ClearOptions();
        
        List<string> options = new List<string>();

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);
        }
        
        resolutionDropdown.AddOptions(options);
    }

    public void SetVolume(float volume)
    {
        audiomixer.SetFloat("volume", volume);
    }

    public void SetQuality (int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void SetFullscreen (bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }
}
