using ExitGames.Client.Photon;
using Photon;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Assets.Scripts.Map_Generation;
using System.Collections;

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

                // Create the player object
                CreateLocalPlayer();

                // Try to load the saved player position
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
        Vector3 position = new Vector3(Random.Range(-10f , 10f), 0f, Random.Range(-10f, 10f));
        LocalPlayer = PhotonNetwork.Instantiate(playerPrefab.name, position, Quaternion.identity, 0);

        OnLocalPlayerCreated?.Invoke(LocalPlayer);

        int photonViewID = LocalPlayer.GetComponent<PhotonView>().viewID;
        PhotonNetwork.RaiseEvent(1, photonViewID, true, null); // Trigger a spawn event so that other players know of our existence.
    }

    /// <summary>
    /// Loads the saved player position and applies it to the LocalPlayer, if no position was loaded then the players Y position will be set to the terrain height.
    /// </summary>
    private void LoadPlayerPosition()
    {
        PlayerDataLoader.PlayerData playerData = PlayerDataLoader.LoadPlayerData(PhotonNetwork.player, SaveDataManager.PlayerDataPath);
        if ((int)PhotonNetwork.player.CustomProperties["UniqueID"] == playerData.Id)
            LocalPlayer.transform.position = playerData.Position;
        else
        {
            if (TerrainGenerator.IsSetupFinished)
                StartCoroutine(PlacePlayerOnTerrain());
            else
                TerrainGenerator.OnSetupFinished += () => { StartCoroutine(PlacePlayerOnTerrain()); };
        }
    }

    private IEnumerator PlacePlayerOnTerrain()
    {
        // This loop is necessary because we don't know when the terrain is actually created.
        while (true)
        {
            yield return new WaitForSeconds(0.1f);

            Vector3 rayStartPos = LocalPlayer.transform.position;
            rayStartPos.y = 10000;

            RaycastHit raycastHitInfo;
            if (Physics.Raycast(new Ray(rayStartPos, Vector3.down), out raycastHitInfo, Mathf.Infinity, TerrainGenerator.LayerMask))
            {
                LocalPlayer.transform.position = raycastHitInfo.point;
                break;
            }
        }

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
