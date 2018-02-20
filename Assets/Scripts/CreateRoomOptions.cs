using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class CreateRoomOptions : MonoBehaviour
{
    [SerializeField] private Text _roomNameText;
    [SerializeField] private Dropdown _playerCountDropdown;
    [SerializeField] private Toggle _roomVisibilityToggle;

    #region Public Methods

    public void OnClick_CreateRoom()
    {
        RoomOptions roomOptions = new RoomOptions()
        {
            IsVisible = _roomVisibilityToggle.isOn,
            MaxPlayers = Convert.ToByte(_playerCountDropdown.options[_playerCountDropdown.value].text),
            IsOpen = true

        };

        if (PhotonNetwork.CreateRoom(_roomNameText.text, roomOptions, TypedLobby.Default))
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
    }

    #endregion //Photon Callbacks
}
