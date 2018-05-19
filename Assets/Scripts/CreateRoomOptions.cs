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
    
    public void OnClick_CreateRoom()
    {
        RoomOptions roomOptions = new RoomOptions()
        {
            CleanupCacheOnLeave = false,
            IsVisible = roomVisibilityDropdown.options[roomVisibilityDropdown.value].text == "PUBLIC" ? true : false,            
            MaxPlayers = Convert.ToByte(playerCountDropdown.options[playerCountDropdown.value].text.Substring(0, 1)),
            IsOpen = true,
            CustomRoomProperties = new ExitGames.Client.Photon.Hashtable() { { "seed", 123456789 } }
        };

        string roomName = string.IsNullOrEmpty(roomNameText?.text) ? PlayerNetwork.PlayerName : roomNameText.text;

        FindObjectOfType<MainMenu>().CreateGame(roomName, roomOptions);
    }
}
