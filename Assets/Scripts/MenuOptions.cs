using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class MenuOptions : MonoBehaviour
{
    public Dropdown qualityDropDown;
    public Dropdown resolutionDropDown;
    public Toggle fullscreenToggle;
    public Text volumeAmount;

    private Resolution[] resolutions;

    private void Start()
    {
        InitializeResolutionDropDown();
        InitializeQualityDropDown();
        InitializeFullscreenToggle();
    }

    /// <summary>
    /// Sets the game volume.
    /// </summary>
    /// <param name="volume">The volume for the game (0.0 to 1.0)</param>
    public void SetVolume(float volume)
    {
        //TODO: split into music and audio volume sliders using the audio mixer.
        AudioListener.volume = volume;
        volumeAmount.text = (volume).ToString("F0") + "%";
    }
    
    public void SetFullScreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    /// <summary>
    /// Sets the screen resolution based on the dropdown
    /// </summary>
    /// <param name="resolutionIndex">The index of the selected dropdown option</param>
    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    /// <summary>
    /// Sets the game quality based on the dropdown
    /// </summary>
    /// <param name="qualityIndex">The index of the selected dropdown option</param>
    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex, true);
    }

    private void InitializeResolutionDropDown()
    {
        resolutions = Screen.resolutions;
        resolutionDropDown.ClearOptions();

        var dropDownResolutions = new List<string>();
        var currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            var option = $"{resolutions[i].width} x {resolutions[i].height} {resolutions[i].refreshRate}Hz";
            dropDownResolutions.Add(option);

            //Check what the current resolution is so we can display it in the dropdown
            if (CompareCurrentResolution(resolutions[i]))
                currentResolutionIndex = i;
        }

        resolutionDropDown.AddOptions(dropDownResolutions);
        resolutionDropDown.value = currentResolutionIndex;
        resolutionDropDown.RefreshShownValue();
    }

    private bool CompareCurrentResolution(Resolution resolution)
    {
        return resolution.width == Screen.width &&
            resolution.height == Screen.height &&
            resolution.refreshRate == Screen.currentResolution.refreshRate;
    }

    private void InitializeQualityDropDown()
    {
        qualityDropDown.ClearOptions();

        var qualityOptions = new List<string>();
        var currentQuality = 0;
        
        for (int i = 0; i < QualitySettings.names.Length; i++)
        {
            qualityOptions.Add(QualitySettings.names[i]);

            if (QualitySettings.GetQualityLevel() == i)
                currentQuality = i;
        }

        qualityDropDown.AddOptions(qualityOptions);
        qualityDropDown.value = currentQuality;
        qualityDropDown.RefreshShownValue();
    }

    private void InitializeFullscreenToggle()
    {
        fullscreenToggle.isOn = Screen.fullScreen;
    }
}
