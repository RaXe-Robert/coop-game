using UnityEngine;
using UnityEngine.UI;

using System.Collections;

public class RoomListing : MonoBehaviour
{
    [SerializeField] private Text nameText;
    [SerializeField] private Text playerCountText;
    [SerializeField] private Button connectButton;

    public string RoomName { get; private set; }
    public bool Updated { get; set; }

    #region MonoBehaviour

    private void Start()
    {
        connectButton?.onClick.AddListener(OnClick);
    }

    #endregion //MonoBehaviour

    #region Private Methods

    private void OnClick()
    {
        if (PhotonNetwork.JoinRoom(RoomName))
            Debug.Log($"Succesfully joined {RoomName}");
        else
            Debug.LogWarning($"Failed to join {RoomName}");
    }

    #endregion //Private Methods

    #region Public Methods

    public void SetRoomName(string text)
    {
        RoomName = text;
        nameText.text = RoomName;
    }

    public void SetPlayerCount(int currentPlayers, int maxPlayers)
    {
        playerCountText.text = $"{currentPlayers}/{maxPlayers}";
    }
    
    #endregion //Public Methods
}
