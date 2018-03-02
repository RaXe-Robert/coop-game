using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject newGameMenuPanel;
    [SerializeField] private GameObject singlePlayerPanel;
    [SerializeField] private GameObject hostGamePanel;
    [SerializeField] private GameObject serverBrowserMenuPanel;
    [SerializeField] private GameObject serverBrowserPanel;
    [SerializeField] private GameObject optionsMenuPanel;
    [SerializeField] private GameObject controlsPanel;
    [SerializeField] private GameObject videoSettingsPanel;
    [SerializeField] private GameObject audioSettingsPanel;
    [SerializeField] private GameObject exitGamePanel;

    private void Start()
    {
        //Initialize volume to 50% so people don't go deaf.
        AudioListener.volume = 0.5f;
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void ChangePlayerName(string name)
    {
        PlayerNetwork.PlayerName = name.Length == 0 ? SystemInfo.deviceName : name;
    }

    #region Panel Navigation

    public void ShowMainMenuPanel(bool state)
    {
        mainMenuPanel.SetActive(state);
    }

    public void ShowNewGameMenuPanel(bool state)
    {
        newGameMenuPanel.SetActive(state);

        if (state)
        {
            ShowSinglePlayerPanel(true);
            ShowMainMenuPanel(false);
        }
    }

    public void ShowServerBrowserMenuPanel(bool state)
    {
        serverBrowserMenuPanel.SetActive(state);

        if (state)
        {
            ShowServerBrowserPanel(true);
            ShowMainMenuPanel(false);
        }
    }

    public void ShowOptionsMenuPanel(bool state)
    {
        optionsMenuPanel.SetActive(state);

        if (state)
        {
            ShowControlsPanel(true);
            ShowMainMenuPanel(false);
        }
    }

    public void ShowSinglePlayerPanel(bool state)
    {
        singlePlayerPanel.SetActive(state);

        if (state)
        {
            ShowHostGamePanel(false);
        }
    }

    public void ShowHostGamePanel(bool state)
    {
        hostGamePanel.SetActive(state);

        if (state)
        {
            ShowSinglePlayerPanel(false);
            ShowServerBrowserPanel(false);
        }
    }

    public void ShowServerBrowserPanel(bool state)
    {
        serverBrowserPanel.SetActive(state);

        if (state)
        {
            ShowHostGamePanel(false);
        }
    }

    public void ShowControlsPanel(bool state)
    {
        controlsPanel.SetActive(state);

        if (state)
        {
            ShowVideoSettingsPanel(false);
            ShowAudioSettingsPanel(false);
        }
    }

    public void ShowVideoSettingsPanel(bool state)
    {
        videoSettingsPanel.SetActive(state);

        if (state)
        {
            ShowControlsPanel(false);
            ShowAudioSettingsPanel(false);
        }
    }

    public void ShowAudioSettingsPanel(bool state)
    {
        audioSettingsPanel.SetActive(state);

        if (state)
        {
            ShowControlsPanel(false);
            ShowVideoSettingsPanel(false);
        }
    }
   
    public void ShowExitGamePanel(bool state)
    {
        exitGamePanel.SetActive(state);
        ShowMainMenuPanel(!state);
    }

    public void HideAllPanelsExceptMain()
    {
        ShowNewGameMenuPanel(false);
        ShowSinglePlayerPanel(false);
        ShowHostGamePanel(false);
        ShowServerBrowserMenuPanel(false);
        ShowServerBrowserPanel(false);
        ShowOptionsMenuPanel(false);
        ShowControlsPanel(false);
        ShowVideoSettingsPanel(false);
        ShowAudioSettingsPanel(false);
        ShowExitGamePanel(false);

        ShowMainMenuPanel(true);
    }

    #endregion // Panel Navigation
}
