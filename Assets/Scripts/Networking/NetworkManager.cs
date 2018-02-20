using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviour
{

    #region MonoBehaviour

    private void Start()
    {
        if (!PhotonNetwork.connected)
        {
            print("Connecting to server...");
            PhotonNetwork.ConnectUsingSettings("v0.1");
        }
    }

    #endregion //MonoBehaviour

    #region Photon Callbacks

    private void OnConnectedToMaster()
    {
        print("Connected to master.");
        PhotonNetwork.automaticallySyncScene = true;
        PhotonNetwork.playerName = "Unassgined";
        PhotonNetwork.JoinLobby(TypedLobby.Default);
    }

    private void OnJoinedLobby()
    {
        print("Joined lobby.");
    }

    #endregion //Photon Callbacks
}
