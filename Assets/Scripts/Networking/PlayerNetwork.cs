using Photon;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerNetwork : PunBehaviour
{
    public static string PlayerName
    {
        get { return PhotonNetwork.player.NickName; }
        set { PhotonNetwork.player.NickName = value; }
    }

    public static GameObject LocalPlayer { get; private set; }

    public static Dictionary<int, PlayerInfo> OtherPlayers { get; private set; }
    public static event System.Action<PhotonView> OtherPlayerSpawned;

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
                    OtherPlayers.Add(photonPlayer.ID, new PlayerInfo(photonPlayer));

                SpawnPlayer();

                /* This is a fix for the player spawning in before the map is actually loaded in multiplayer, some of the UI stuff that is dependent on the player breaks when using this tho.
                if (SaveDataManager.Instance.IsWorldDownloaded)
                    SpawnPlayer();
                else
                    SaveDataManager.Instance.OnWorldDownloaded += () => { SpawnPlayer(); };
                    */
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

    public void SpawnPlayer()
    {
        Vector3 position = new Vector3(Random.Range(-10f , 10f), 0.2f, Random.Range(-10f, 10f));
        LocalPlayer = PhotonNetwork.Instantiate("Player", position, Quaternion.identity, 0);

        int photonViewID = LocalPlayer.GetComponent<PhotonView>().viewID;
        PhotonNetwork.RaiseEvent(1, photonViewID, true, null);
    }

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

                OtherPlayerSpawned?.Invoke(playerPhotonView);
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
