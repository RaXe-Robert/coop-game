using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviour
{
    private void Start()
    {}

    public bool Connected { get { return PhotonNetwork.connected; } }
    public bool OfflineMode { get { return PhotonNetwork.offlineMode; } }

    /// <summary>
    /// Connects photon, will skip if already connected
    /// </summary>
    public void Connect()
    {
        print("NM Connect");
        if (!PhotonNetwork.connected)
        {
            print("Connecting to server...");
            PhotonNetwork.ConnectUsingSettings("v0.1");
        }
    }

    /// <summary>
    /// Disconnects photon, will skip if already disconnected
    /// </summary>
    public void Disconnect()
    {
        print("NM Disconnect");
        if (PhotonNetwork.connected)
        {
            PhotonNetwork.Disconnect();
            print("Disconnected");
        }
    }

    /// <summary>
    /// Sets the offline mode of photon, will throw if photon is already connected
    /// </summary>
    /// <param name="state"></param>
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
    }

    private void OnJoinedLobby()
    {
        print("Joined lobby.");
    }

    #endregion //Photon Callbacks
}