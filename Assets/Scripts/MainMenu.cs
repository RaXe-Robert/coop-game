using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject newGameMenuPanel;

    [SerializeField] private GameObject loadSingleplayerPanel;
    [SerializeField] private GameObject createSingleplayerPanel;

    [SerializeField] private GameObject loadMultiplayerPanel;
    [SerializeField] private GameObject createMultiplayerPanel;

    [SerializeField] private GameObject serverBrowserMenuPanel;
    [SerializeField] private GameObject serverBrowserPanel;

    [SerializeField] private GameObject optionsMenuPanel;
    [SerializeField] private GameObject controlsPanel;
    [SerializeField] private GameObject videoSettingsPanel;
    [SerializeField] private GameObject audioSettingsPanel;

    [SerializeField] private GameObject exitGamePanel;

    [Header("Player name")]
    [SerializeField] private GameObject enterNamePanel;
    [SerializeField] private InputField inputNameText;
    [SerializeField] private Text nameText;

    [Header("Error message")]
    [SerializeField] private GameObject errorMessagePanel;
    [SerializeField] private Text errorMessage;

    private Stack<GameObject> menuStack = new Stack<GameObject>();
    private NetworkManager networkManager;

    private void Awake()
    {
        //Initialize volume to 50% so people don't go deaf.
        AudioListener.volume = 0.5f;
    }

    private void Start()
    {
        //TODO: There should be a better way to get the network manager
        networkManager = FindObjectOfType<NetworkManager>();

        nameText.text = PlayerPrefs.GetString("PlayerName");
        
        // Create a random id for players so that servers can recognize certain players (this will need to be replaced by an account database if we get that far)
        int playerId = PlayerPrefs.GetInt("UniqueID", -1);
        if (playerId == -1)
        {
            playerId = (new System.Random(System.Guid.NewGuid().GetHashCode())).Next(0, int.MaxValue);
            PlayerPrefs.SetInt("UniqueID", playerId);
        }
        PhotonNetwork.player.CustomProperties["UniqueID"] = playerId;

        //When the player returns from the game to the main menu, the photon is still connected
        if (networkManager.Connected)
            networkManager.Disconnect();

        //Sets the menu stack
        menuStack.Clear();
        menuStack.Push(mainMenuPanel);
    }

    public void CreateGame(string roomName, RoomOptions roomOptions, bool offlineMode)
    {
        PhotonNetwork.offlineMode = offlineMode;

        if (!PhotonNetwork.connected)
            OnPhotonCreateRoomFailed(new object[] { 1, "Not connected to master server!" });
        else if(!PhotonNetwork.CreateRoom(roomName, roomOptions, TypedLobby.Default))
            OnPhotonCreateRoomFailed(new object[] { 2, "Room with the same name already exists!" });
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
        ShowErrorMessagePanel(codeAndMessage[0].ToString(), codeAndMessage[1].ToString());
    }

    private void OnCreatedRoom()
    {
        print("Succesfully created a room");
        PhotonNetwork.LoadLevel("Game");
    }

    #endregion //Photon Callbacks

    #region Panel Navigation

    public void ReturnToPreviousMenu(int levels = 1)
    {
        for (int i = levels; i > 0; --i)
            CloseMenu();
    }

    public void ShowNewGameMenuPanel()
    {
        OpenMenu(newGameMenuPanel);

        //First menu shown is singleplayer, so connect in offline mode
        Connect(offlineMode: true);
        OpenMenu(loadSingleplayerPanel, disablePrevious: false);
    }

    public void ShowServerBrowserMenuPanel()
    {
        OpenMenu(serverBrowserMenuPanel);

        Connect();
        OpenMenu(serverBrowserPanel, disablePrevious: false);
    }

    public void ShowOptionsMenuPanel()
    {
        OpenMenu(optionsMenuPanel);
        OpenMenu(controlsPanel, disablePrevious: false);
    }

    public void ShowLoadSingleplayerPanel()
    {
        Connect(offlineMode: true);
        CloseMenu();
        OpenMenu(loadSingleplayerPanel, disablePrevious: false);
    }

    public void ShowCreateSingleplayerPanel()
    {
        CloseMenu();
        OpenMenu(createSingleplayerPanel, disablePrevious: false);
    }

    public void ShowLoadMupltiplayerGamePanel()
    {
        Connect();
        CloseMenu();
        OpenMenu(loadMultiplayerPanel, disablePrevious: false);
    }

    public void ShowCreateMultiplayerPanel()
    {
        CloseMenu();
        OpenMenu(createMultiplayerPanel, disablePrevious: false);
    }

    public void ShowServerBrowserPanel()
    {
        Connect();
        CloseMenu();
        OpenMenu(serverBrowserPanel, disablePrevious: false);
    }

    public void ShowControlsPanel()
    {
        CloseMenu();
        OpenMenu(controlsPanel, disablePrevious: false);
    }

    public void ShowVideoSettingsPanel()
    {
        CloseMenu();
        OpenMenu(videoSettingsPanel, disablePrevious: false);
    }

    public void ShowAudioSettingsPanel()
    {
        CloseMenu();
        OpenMenu(audioSettingsPanel, disablePrevious: false);
    }

    public void ShowExitGamePanel()
    {
        OpenMenu(exitGamePanel, disablePrevious: false);
    }

    public void ShowEnterNamePanel()
    {
        OpenMenu(enterNamePanel, disablePrevious: false);
    }

    public void DoneEnterName()
    {
        if(inputNameText.text.EndsWith(" ") || inputNameText.text.Length < 2)
        {
            ShowErrorMessagePanel("Well Well Well.", "Your name has to be more than one character and contain no spaces at the end.");
            return;
        }
        ChangePlayerName(inputNameText.text);
        UpdatePlayerName();
        CloseMenu();
    }

    private void ShowErrorMessagePanel(string code, string message)
    {
        errorMessage.text = $"[{code}] {message}";
        OpenMenu(errorMessagePanel, disablePrevious: false);
        print($"Error: [{code}] { message}");
    }

    public void CloseErrorMessagePanel()
    {
        CloseMenu();
    }

    private void UpdatePlayerName()
    {
        nameText.text = PlayerNetwork.PlayerName;
    }

    /// <summary>
    /// Activates a menu and adds it to the menu stack
    /// </summary>
    /// <param name="menu">The next window to open</param>
    /// <param name="disablePrevious">Whether the previous menu needs to be closed</param>
    private void OpenMenu(GameObject menu, bool disablePrevious = true)
    {
        //Check is the current menu is already in the stack to prevent duplicates
        if (menuStack.Contains(menu))
            return;

        //Retrieves the previous menu and pop and deactivate
        var prev = menuStack.Peek();
        if (prev != null && disablePrevious)
            prev.SetActive(false);

        //Add the next menu to the stack and enable it
        menuStack.Push(menu);
        menu.SetActive(true);
    }

    /// <summary>
    /// Closes the current menu and pops it from the menu stack
    /// </summary>
    /// <param name="enablePrevious">Whether the previous menu needs to be shown or not</param>
    private void CloseMenu(bool enablePrevious = true)
    {
        var current = menuStack.Pop();
        current.SetActive(false);

        var prev = menuStack.Peek();
        if (prev != null && enablePrevious)
            prev.SetActive(true);
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
            if (networkManager.Connected)
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
