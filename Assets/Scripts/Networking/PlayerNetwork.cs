using ExitGames.Client.Photon;
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

    private List<int> availableColors = new List<int> { 0,1,2,3,4,5,6 };

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
                foreach (var photonPlayer in PhotonNetwork.otherPlayers)
                {
                    availableColors.Remove((int)photonPlayer.CustomProperties["Color"]);
                    OtherPlayers.Add(photonPlayer.ID, new PlayerInfo(photonPlayer));
                }
                int color = availableColors[Random.Range(0, availableColors.Count)];
                Hashtable colorHashtable = new Hashtable() { { "Color", color } };
                PhotonNetwork.SetPlayerCustomProperties(colorHashtable);

                CreateLocalPlayer(color);

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
    private void CreateLocalPlayer(int color)
    {
        Vector3 position = new Vector3(Random.Range(-10f , 10f), 20f, Random.Range(-10f, 10f));
        LocalPlayer = PhotonNetwork.Instantiate(playerPrefab.name, position, Quaternion.identity, 0);
        SetPlayerColor(LocalPlayer,color);
        int photonViewID = LocalPlayer.GetComponent<PhotonView>().viewID;
        PhotonNetwork.RaiseEvent(1, photonViewID, true, null); // Trigger a spawn event so that other players know of our existence.
    }

    public void SetPlayerColor(GameObject gameObject,int color)
    {
        switch (color)
        {
            case 0://red
                gameObject.GetComponentInChildren<SkinnedMeshRenderer>().materials[3].color = new Color32(255, 0, 0, 100);
                break;
            case 1://orange
                gameObject.GetComponentInChildren<SkinnedMeshRenderer>().materials[3].color = new Color32(255, 127, 0, 100);
                break;
            case 2://yellow
                gameObject.GetComponentInChildren<SkinnedMeshRenderer>().materials[3].color = new Color32(255, 255, 0, 100);
                break;
            case 3://green
                gameObject.GetComponentInChildren<SkinnedMeshRenderer>().materials[3].color = new Color32(0, 255, 0, 100);
                break;
            case 4://blue
                gameObject.GetComponentInChildren<SkinnedMeshRenderer>().materials[3].color = new Color32(0, 0, 255, 100);
                break;
            case 5://indigo
                gameObject.GetComponentInChildren<SkinnedMeshRenderer>().materials[3].color = new Color32(75, 0, 130, 100);
                break;
            case 6://purple
                gameObject.GetComponentInChildren<SkinnedMeshRenderer>().materials[3].color = new Color32(143, 0, 255, 100);
                break;
        }
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
                {
                    OtherPlayers[playerPhotonView.ownerId].GameObject = playerPhotonView.gameObject;
                    Debug.Log(OtherPlayers[playerPhotonView.ownerId].PlayerColor);
                    SetPlayerColor(OtherPlayers[playerPhotonView.ownerId].GameObject, OtherPlayers[playerPhotonView.ownerId].PlayerColor);
                }
                else
                {
                    OtherPlayers.Add(playerPhotonView.ownerId, new PlayerInfo(PhotonPlayer.Find(playerPhotonView.ownerId)) { GameObject = playerPhotonView.gameObject });
                    SetPlayerColor(OtherPlayers[playerPhotonView.ownerId].GameObject, OtherPlayers[playerPhotonView.ownerId].PlayerColor);
                }
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
    public int PlayerColor;

    public PlayerInfo(PhotonPlayer photonPlayer)
    {
        this.PhotonPlayer = photonPlayer;
        this.PlayerColor = (int)photonPlayer.CustomProperties["Color"];
    }
}
