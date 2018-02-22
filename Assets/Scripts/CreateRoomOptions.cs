using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class CreateRoomOptions : MonoBehaviour
{
    [SerializeField] private Text roomNameText;
    [SerializeField] private Dropdown playerCountDropdown;
    [SerializeField] private Dropdown roomVisibilityDropdown;

    #region Public Methods

    public void OnClick_CreateRoom()
    {
        RoomOptions roomOptions = new RoomOptions()
        {

            IsVisible = roomVisibilityDropdown.options[roomVisibilityDropdown.value].text == "PUBLIC" ? true : false,
            MaxPlayers = Convert.ToByte(playerCountDropdown.options[playerCountDropdown.value].text.Substring(0, 1)),
            IsOpen = true
        };

        if (PhotonNetwork.CreateRoom(roomNameText.text, roomOptions, TypedLobby.Default))
        {
            print("create room succesfully sent.");
        }
        else
        {
            print("create room failed to send");
        }
    }

    #endregion //Public Methods

    #region Photon Callbacks

    private void OnPhotonCreateRoomFailed(object[] codeAndMessage)
    {
        print("create room failed: " + codeAndMessage[1]);
    }

    private void OnCreatedRoom()
    {
        print("Room created succesfully.");

        PhotonNetwork.LoadLevel("Game");
    }

    #endregion //Photon Callbacks
}
