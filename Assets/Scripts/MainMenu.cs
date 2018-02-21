using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

    public GameObject mainMenuPanel;

    public GameObject newGameMenuPanel;
    public GameObject singlePlayerPanel;
    public GameObject hostGamePanel;

    public GameObject serverBrowserMenuPanel;
    public GameObject serverBrowserPanel;

    public GameObject optionsMenuPanel;

    public GameObject exitGamePanel;




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
    }

    public void HideHostGamePanel()
    {
        hostGamePanel.SetActive(false);
    }

    public void ShowServerBrowserPanel()
    {
        serverBrowserPanel.SetActive(true);
    }

    public void HideServerBrowserPanel()
    {
        serverBrowserPanel.SetActive(false);
    }
    
    public void ShowOptionsMenu()
    {
        optionsMenuPanel.SetActive(true);
    }

    public void HideOptionsMenu()
    {
        optionsMenuPanel.SetActive(false);
    }
   
    public void ShowExitGamePanel()
    {
        exitGamePanel.SetActive(true);
    }

    public void HideExitGamePanel()
    {
        exitGamePanel.SetActive(false);
    }

    public void HideAllPanelsExceptMain()
    {
        mainMenuPanel.SetActive(true);
        newGameMenuPanel.SetActive(false);
        singlePlayerPanel.SetActive(false);
        hostGamePanel.SetActive(false);
        serverBrowserMenuPanel.SetActive(false);
        optionsMenuPanel.SetActive(false);
        exitGamePanel.SetActive(false);
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
