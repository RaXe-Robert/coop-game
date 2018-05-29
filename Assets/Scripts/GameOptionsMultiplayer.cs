using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

/// <summary>
/// Configurable game options for the user to create a game room.
/// </summary>
public class GameOptionsMultiplayer : GameOptionsSingleplayer
{
    [Header("Multiplayer settings")]
    [SerializeField] private Text roomNameText;
    [SerializeField] private Dropdown playerCountDropdown;
    [SerializeField] private Dropdown roomVisibilityDropdown;
    
    public override void OnClick_CreateGame()
    {
        RoomOptions roomOptions = new RoomOptions()
        {
            CleanupCacheOnLeave = false,
            IsVisible = roomVisibilityDropdown.options[roomVisibilityDropdown.value].text == "PUBLIC" ? true : false,            
            MaxPlayers = Convert.ToByte(playerCountDropdown.options[playerCountDropdown.value].text.Substring(0, 1)),
            IsOpen = true,
            CustomRoomProperties = new ExitGames.Client.Photon.Hashtable()
            {
                { "seed", Convert.ToInt32(seedText.text) },
                { "saveName", saveNameText.text}
            }
        };

        string roomName = string.IsNullOrEmpty(roomNameText?.text) ? PlayerNetwork.PlayerName : roomNameText.text;

        FindObjectOfType<MainMenu>().CreateGame(roomName, roomOptions, false);
    }

    public override void OnClick_LoadGame()
    {
        SaveFileBrowser saveFileBrowser = GetComponent<SaveFileBrowser>();
        if (saveFileBrowser == null)
            Debug.LogError("Tried to load game but no savefilebrowser attached to this gameobject.");
        else if (saveFileBrowser.SelectedSave == null)
            Debug.LogError("Tried to load save but no save file is selected.");
        else
        {
            RoomOptions roomOptions = new RoomOptions()
            {
                CleanupCacheOnLeave = false,
                IsVisible = true,
                MaxPlayers = 4,
                IsOpen = true,
                CustomRoomProperties = new ExitGames.Client.Photon.Hashtable()
                {
                    { "seed", Convert.ToInt32(saveFileBrowser.SelectedSave.Seed) },
                    { "saveName", saveFileBrowser.SelectedSave.Name }
                }
            };

            FindObjectOfType<MainMenu>().CreateGame(saveFileBrowser.SelectedSave.Name, roomOptions, false);
        }
    }
}
