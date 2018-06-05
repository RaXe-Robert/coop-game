using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MenuOptions : MonoBehaviour
{
    public Dropdown qualityDropDown;
    public Dropdown resolutionDropDown;
    public Toggle fullscreenToggle;

    private Resolution[] resolutions;

    [SerializeField]
    private Slider masterVolume, musicVolume, ambienceVolume, soundVolume;

    [SerializeField]
    private AudioMixer masterMixer;

    private void Awake()
    {
        // Load settings
        SetMasterVolume(PlayerPrefs.GetFloat("masterVol", 0));
        SetMusicVolume(PlayerPrefs.GetFloat("musicVol", 0));
        SetAmbienceVolume(PlayerPrefs.GetFloat("ambienceVol", 0));
        SetSoundVolume(PlayerPrefs.GetFloat("soundVol", 0));
    }

    private void Start()
    {
        InitializeResolutionDropDown();
        InitializeQualityDropDown();
        InitializeFullscreenToggle();
    }

    /// <summary>
    /// Sets the master volume.
    /// </summary>
    public void SetMasterVolume(float volume)
    {
        masterMixer.SetFloat("masterVol", volume);
        PlayerPrefs.SetFloat("masterVol", volume);

        masterVolume.value = volume > 20 ? 20 : volume; // Safety to prevent it from getting extremely loud
    }

    /// <summary>
    /// Sets the music volume.
    /// </summary>
    public void SetMusicVolume(float volume)
    {
        masterMixer.SetFloat("musicVol", volume);
        PlayerPrefs.SetFloat("musicVol", volume);

        musicVolume.value = volume > 20 ? 20 : volume; // Safety to prevent it from getting extremely loud
    }

    /// <summary>
    /// Sets the ambience volume.
    /// </summary>
    public void SetAmbienceVolume(float volume)
    {
        masterMixer.SetFloat("ambienceVol", volume);
        PlayerPrefs.SetFloat("ambienceVol", volume);

        ambienceVolume.value = volume > 20 ? 20 : volume; // Safety to prevent it from getting extremely loud
    }

    /// <summary>
    /// Sets the sounds volume.
    /// </summary>
    public void SetSoundVolume(float volume)
    {
        masterMixer.SetFloat("soundVol", volume);
        PlayerPrefs.SetFloat("soundVol", volume);

        soundVolume.value = volume > 20 ? 20 : volume; // Safety to prevent it from getting extremely loud
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
