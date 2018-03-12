using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviour
{
    private void Start()
    {}

    public bool Connected { get { return PhotonNetwork.connected; } }
    public bool OfflineMode { get { return PhotonNetwork.offlineMode; } }

    public void Connect()
    {
        if (!PhotonNetwork.connected)
        {
            print("Connecting to server...");
            PhotonNetwork.ConnectUsingSettings("v0.1");
        }
    }

    public void Disconnect()
    {
        if (PhotonNetwork.connected)
        {
            PhotonNetwork.Disconnect();
            print("Disconnected");
        }
    }

    public void SetOfflineMode(bool state)
    {
        if (PhotonNetwork.connected)
            throw new System.Exception("Can't change offline mode while connected");

        PhotonNetwork.offlineMode = state;
    }

    #region Photon Callbacks

    private void OnConnectedToMaster()
    {
        print($"Connected to master. (Offline mode = {PhotonNetwork.offlineMode})");
        PhotonNetwork.automaticallySyncScene = true;
        PlayerNetwork.PlayerName = SystemInfo.deviceName;

        if (PhotonNetwork.offlineMode)
            return;

        PhotonNetwork.JoinLobby(TypedLobby.Default);

#if UNITY_EDITOR
        if (UnityEditor.EditorPrefs.GetBool("AutoStartRoom"))
        {
            return;
            RoomOptions roomOptions = new RoomOptions()
            {
                CleanupCacheOnLeave = false,
                IsVisible = true,
                MaxPlayers = 0,
                IsOpen = true
            };

            string roomName = "Testing";

            if (PhotonNetwork.CreateRoom(roomName, roomOptions, TypedLobby.Default))
            {
                print("Succesfully requested a room");
            }
            else
            {
                print("Failed to send a room request!");
            }
        }
#endif
    }

    private void OnJoinedLobby()
    {
        print("Joined lobby.");
    }

    #endregion //Photon Callbacks
}