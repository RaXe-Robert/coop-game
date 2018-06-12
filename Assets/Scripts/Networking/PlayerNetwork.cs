using Photon;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerNetwork : PunBehaviour
{
    [SerializeField] private GameObject playerPrefab;

    public static string PlayerName
    {
        get { return PhotonNetwork.player.NickName; }
        set { PhotonNetwork.player.NickName = value; }
    }

    public static GameObject LocalPlayer { get; private set; }
    public static event System.Action<GameObject> OnLocalPlayerCreated;

    public static Dictionary<int, PlayerInfo> OtherPlayers { get; private set; }
    public static event System.Action<PhotonView> OnOtherPlayerCreated;

    private void Awake()
    {  
        SceneManager.sceneLoaded += OnSceneFinishedLoading;

        PhotonNetwork.OnEventCall += PlayerSpawnEvent;
    }

    private void OnSceneFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Game")
        {
            if (PhotonNetwork.inRoom)
            {
                // Collect all player objects
                PlayerInputController[] playerInputControllers = FindObjectsOfType<PlayerInputController>();

                foreach (var photonPlayer in PhotonNetwork.otherPlayers)
                {
                    GameObject playerObject = null;
                    foreach (var playerInputController in playerInputControllers)
                    {
                        if (photonPlayer.ID == playerInputController.gameObject.GetComponent<PhotonView>().ownerId)
                        {
                            playerObject = playerInputController.gameObject;
                            break;
                        }
                    }

                    OtherPlayers.Add(photonPlayer.ID, new PlayerInfo(photonPlayer) { GameObject = playerObject });
                }

                CreateLocalPlayer();

                if (SaveDataManager.Instance.SaveFilesDownloaded)
                    LoadPlayerPosition();
                else
                    SaveDataManager.Instance.OnSaveFilesDownloaded += LoadPlayerPosition;
            }
            else
                Debug.LogError("Trying to spawn but player is not in room!");
        }
        else if (scene.name == "MainMenu")
        {
            LocalPlayer = null;
            OtherPlayers = new Dictionary<int, PlayerInfo>();
        }
    }

    /// <summary>
    /// Creates the local player object. This only happens once.
    /// </summary>
    private void CreateLocalPlayer()
    {
        Vector3 position = new Vector3(Random.Range(-10f , 10f), 20f, Random.Range(-10f, 10f));
        LocalPlayer = PhotonNetwork.Instantiate(playerPrefab.name, position, Quaternion.identity, 0);

        int photonViewID = LocalPlayer.GetComponent<PhotonView>().viewID;
        PhotonNetwork.RaiseEvent(1, photonViewID, true, null); // Trigger a spawn event so that other players know of our existence.
    }

    /// <summary>
    /// Loads the saved player position and applies it to the LocalPlayer.
    /// </summary>
    private void LoadPlayerPosition()
    {
        PlayerDataLoader.PlayerData playerData = PlayerDataLoader.LoadPlayerData(PhotonNetwork.player, SaveDataManager.PlayerDataPath);
        if ((int)PhotonNetwork.player.CustomProperties["UniqueID"] == playerData.Id)
            LocalPlayer.transform.position = playerData.Position;
    }

    #region Photon

    private void PlayerSpawnEvent(byte eventcode, object content, int senderid)
    {
        if (eventcode == 1)
        {
            int photonViewID = (int)content;

            PhotonView playerPhotonView = PhotonView.Find(photonViewID);
            if (playerPhotonView != null)
            {
                if (OtherPlayers.ContainsKey(playerPhotonView.ownerId))
                    OtherPlayers[playerPhotonView.ownerId].GameObject = playerPhotonView.gameObject;
                else
                    OtherPlayers.Add(playerPhotonView.ownerId, new PlayerInfo(PhotonPlayer.Find(playerPhotonView.ownerId)) { GameObject = playerPhotonView.gameObject });

                OnOtherPlayerCreated?.Invoke(playerPhotonView);
            }
        }
    }

    public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
    {
        if (!OtherPlayers.ContainsKey(newPlayer.ID))
            OtherPlayers.Add(newPlayer.ID, new PlayerInfo(newPlayer));

        // Refresh our photon player object on all other players
        PhotonNetwork.RaiseEvent(1, LocalPlayer.GetPhotonView().viewID, true, null);
    }

    public override void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
    {
        if (OtherPlayers.ContainsKey(otherPlayer.ID))
            OtherPlayers.Remove(otherPlayer.ID);
    }

    #endregion //Photon
}

public class PlayerInfo
{
    public readonly PhotonPlayer PhotonPlayer;
    public GameObject GameObject { get; set; }

    public PlayerInfo(PhotonPlayer photonPlayer)
    {
        this.PhotonPlayer = photonPlayer;
    }
}
