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

    public void ShowMainMenuPanel()
    {
        mainMenuPanel.SetActive(true);
    }

    public void HideMainMenuPanel()
    {
        mainMenuPanel.SetActive(false);
    }

    public void ShowNewGameMenuPanel()
    {
        newGameMenuPanel.SetActive(true);
        ShowSinglePlayerPanel();
        HideMainMenuPanel();
    }

    public void HideNewGameMenuPanel()
    {
        newGameMenuPanel.SetActive(false);
    }

    public void ShowServerBrowserMenuPanel()
    {
        serverBrowserMenuPanel.SetActive(true);
        ShowServerBrowserPanel();
        HideMainMenuPanel();
    }

    public void HideServerBrowserMenuPanel()
    {
        serverBrowserMenuPanel.SetActive(false);
    }

    public void ShowOptionsMenuPanel()
    {
        optionsMenuPanel.SetActive(true);
        ShowControlsPanel();
        HideMainMenuPanel();
    }

    public void HideOptionsMenuPanel()
    {
        optionsMenuPanel.SetActive(false);
    }

    public void ShowSinglePlayerPanel()
    {
        singlePlayerPanel.SetActive(true);
        HideHostGamePanel();
    }

    public void HideSinglePlayerPanel()
    {
        singlePlayerPanel.SetActive(false);
    }

    public void ShowHostGamePanel()
    {
        hostGamePanel.SetActive(true);
        HideSinglePlayerPanel();
        HideServerBrowserPanel();
    }

    public void HideHostGamePanel()
    {
        hostGamePanel.SetActive(false);
    }

    public void ShowServerBrowserPanel()
    {
        serverBrowserPanel.SetActive(true);
        HideHostGamePanel();
    }

    public void HideServerBrowserPanel()
    {
        serverBrowserPanel.SetActive(false);
    }

    public void ShowControlsPanel()
    {
        controlsPanel.SetActive(true);
        HideVideoSettingsPanel();
        HideAudioSettingsPanel();
    }

    public void HideControlsPanel()
    {
        controlsPanel.SetActive(false);
    }

    public void ShowVideoSettingsPanel()
    {
        videoSettingsPanel.SetActive(true);
        HideControlsPanel();
        HideAudioSettingsPanel();
    }

    public void HideVideoSettingsPanel()
    {
        videoSettingsPanel.SetActive(false);
    }

    public void ShowAudioSettingsPanel()
    {
        audioSettingsPanel.SetActive(true);
        HideControlsPanel();
        HideVideoSettingsPanel();
    }

    public void HideAudioSettingsPanel()
    {
        audioSettingsPanel.SetActive(false);
    }
   
    public void ShowExitGamePanel()
    {
        exitGamePanel.SetActive(true);
        HideMainMenuPanel();
    }

    public void HideExitGamePanel()
    {
        exitGamePanel.SetActive(false);
        ShowMainMenuPanel();
    }

    public void HideAllPanelsExceptMain()
    {
        ShowMainMenuPanel();
        HideNewGameMenuPanel();
        HideSinglePlayerPanel();
        HideHostGamePanel();
        HideServerBrowserMenuPanel();
        HideServerBrowserPanel();
        HideOptionsMenuPanel();
        HideControlsPanel();
        HideVideoSettingsPanel();
        HideAudioSettingsPanel();
        HideExitGamePanel();        
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
