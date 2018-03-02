using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviour
{
    private void Start()
    {
        if (!PhotonNetwork.connected)
        {
            print("Connecting to server...");
            PhotonNetwork.ConnectUsingSettings("v0.1");
        }
    }

    #region Photon Callbacks

    private void OnConnectedToMaster()
    {
        print("Connected to master.");
        PhotonNetwork.automaticallySyncScene = true;
        PlayerNetwork.PlayerName = SystemInfo.deviceName;
        PhotonNetwork.JoinLobby(TypedLobby.Default);

#if UNITY_EDITOR
        if (UnityEditor.EditorPrefs.GetBool("AutoStartRoom"))
        {
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

#if UNITY_EDITOR
    private void OnCreatedRoom()
    {
        print("Succesfully created a room");
        PhotonNetwork.LoadLevel("Game");
    }
#endif

    private void OnJoinedLobby()
    {
        print("Joined lobby.");
    }

    #endregion //Photon Callbacks
}