using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class CreateRoomOptions : MonoBehaviour
{
    [SerializeField] private Text roomNameText;
    [SerializeField] private Dropdown playerCountDropdown;
    [SerializeField] private Dropdown roomVisibilityDropdown;
    [SerializeField] private Dropdown objectDropdown;

    #region Public Methods
    public void OnClick_CreateRoom()
    {
        RoomOptions roomOptions = new RoomOptions()
        {
            CleanupCacheOnLeave = objectDropdown.options[objectDropdown.value].text == "SERVER BOUND" ? false : true,
            IsVisible = roomVisibilityDropdown.options[roomVisibilityDropdown.value].text == "PUBLIC" ? true : false,            
            MaxPlayers = Convert.ToByte(playerCountDropdown.options[playerCountDropdown.value].text.Substring(0, 1)),
            IsOpen = true            
        };

        if (PhotonNetwork.CreateRoom(roomNameText.text, roomOptions, TypedLobby.Default))
        {
            print("Succesfully requested a room");
        }
        else
        {
            print("Failed to send a room request!");
        }
    }
    #endregion //Public Methods

    #region Photon Callbacks

    private void OnPhotonCreateRoomFailed(object[] codeAndMessage)
    {
        print("Failed to create a room: " + codeAndMessage[1]);
    }

    private void OnCreatedRoom()
    {
        print("Succesfully created a room");        
        PhotonNetwork.LoadLevel("Game");
    }

    #endregion //Photon Callbacks
}
