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

    private NetworkManager networkManager;

    private void Start()
    {
        //Initialize volume to 50% so people don't go deaf.
        AudioListener.volume = 0.5f;

        //TODO: There should be a better way to get the netweork manager
        networkManager = FindObjectOfType<NetworkManager>();
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void StartSinglePlayerGame()
    {
        PhotonNetwork.offlineMode = true;

        //TODO Move all room creation scripts to the NetworkManager
        RoomOptions options = new RoomOptions()
        {
            IsOpen = false,
            MaxPlayers = 1,
            IsVisible = false,
            CleanupCacheOnLeave = true
        };
        PhotonNetwork.CreateRoom("Singleplayer Game", options, TypedLobby.Default);

        StartGame();
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
        print("ShowSingle");
        Connect(true);

        singlePlayerPanel.SetActive(state);

        if (state)
        {
            ShowHostGamePanel(false);
        }
    }

    public void ShowHostGamePanel(bool state)
    {
        Connect();

        hostGamePanel.SetActive(state);

        if (state)
        {
            ShowSinglePlayerPanel(false);
            ShowServerBrowserPanel(false);
        }
    }

    public void ShowServerBrowserPanel(bool state)
    {
        Connect();

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
        //Disconnect();

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

    private void Connect(bool offlineMode = false)
    {
        print("Menu Connect");
        if (networkManager.Connected)
            Disconnect();

        networkManager.SetOfflineMode(offlineMode);
        networkManager.Connect();
    }

    private void Disconnect()
    {
        print("Menu Disconnect");
        if (networkManager.Connected)
            networkManager.Disconnect();
    }

    #endregion // Panel Navigation
}
