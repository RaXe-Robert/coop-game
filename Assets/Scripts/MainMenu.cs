using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
    [SerializeField] private GameObject enterNamePanel;
    [SerializeField] private InputField inputNameText;
    [SerializeField] private Text nameText;


    private NetworkManager networkManager;

    private void Start()
    {
        //Initialize volume to 50% so people don't go deaf.
        AudioListener.volume = 0.5f;

        //TODO: There should be a better way to get the network manager
        networkManager = FindObjectOfType<NetworkManager>();

        nameText.text = PlayerPrefs.GetString("PlayerName");

        //When the player returns from the game to the main menu, the photon is still connected
        if (networkManager.Connected)
            networkManager.Disconnect();
    }

    public void StartSinglePlayerGame()
    {
        PhotonNetwork.offlineMode = true;

        //TODO Move all room creation scripts to the NetworkManager
        RoomOptions roomOptions = new RoomOptions()
        {
            IsOpen = false,
            MaxPlayers = 1,
            IsVisible = false,
            CleanupCacheOnLeave = true
        };
        CreateGame("Singleplayer Game", roomOptions);
    }

    public void CreateGame(string roomName, RoomOptions roomOptions)
    {
        PhotonNetwork.CreateRoom(roomName, roomOptions, TypedLobby.Default);
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
        if (name == string.Empty)
            return;

        PlayerPrefs.SetString("PlayerName", name);
        PlayerNetwork.PlayerName = name;
    }

    #region Photon Callbacks

    private void OnPhotonCreateRoomFailed(object[] codeAndMessage)
    {
        print("Failed to create a room: " + codeAndMessage[1]);
    }

    private void OnCreatedRoom()
    {
        print("Succesfully created a room");
        PhotonNetwork.LoadLevel("Game");
    }

    #endregion //Photon Callbacks

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
            Connect(true);
            ShowHostGamePanel(false);
        }
    }

    public void ShowHostGamePanel(bool state)
    {
        hostGamePanel.SetActive(state);

        if (state)
        {
            Connect();
            ShowSinglePlayerPanel(false);
            ShowServerBrowserPanel(false);
        }
    }

    public void ShowServerBrowserPanel(bool state)
    {
        serverBrowserPanel.SetActive(state);

        if (state)
        {
            Connect();
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

    public void ShowEnterNamePanel(bool state)
    {
        enterNamePanel.SetActive(state);    
    }    

    public void DoneEnterName()
    {
        ChangePlayerName(inputNameText.text);
        UpdatePlayerName();
        ShowEnterNamePanel(false);
    }

    private void UpdatePlayerName()
    {
        nameText.text = PlayerNetwork.PlayerName;
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

    /// <summary>
    /// Connects to photon via the NetworkManager if photon is not connected
    /// </summary>
    /// <param name="offlineMode">Whether it should connect in offline mode or not</param>
    private void Connect(bool offlineMode = false)
    {
        //If connected to photon and offline mode is not correct, disconnect and reconnect
        if (!networkManager.Connected || networkManager.OfflineMode != offlineMode)
        {
            if(networkManager.Connected)
                Disconnect();
            networkManager.SetOfflineMode(offlineMode);
            networkManager.Connect();
        }
    }

    /// <summary>
    /// Disconnects the NetworkManager is it is conencted
    /// </summary>
    private void Disconnect()
    {
        if (networkManager.Connected)
            networkManager.Disconnect();
    }

    #endregion // Panel Navigation
}
