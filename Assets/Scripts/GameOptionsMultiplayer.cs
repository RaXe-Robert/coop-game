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
    protected string RoomNameInput => !string.IsNullOrWhiteSpace(roomNameText?.text) ? roomNameText.text : string.Empty;

    [SerializeField] private Dropdown playerCountDropdown;
    protected byte MaxPlayerCountInput => playerCountDropdown != null ? Convert.ToByte(playerCountDropdown.options[playerCountDropdown.value].text.Substring(0, 1)) : (byte)4; // If there is no dropdown then the default value is 4

    [SerializeField] private Dropdown roomVisibilityDropdown;
    protected bool RoomVisibleInput => roomVisibilityDropdown != null ? roomVisibilityDropdown.options[roomVisibilityDropdown.value].text == "PUBLIC" ? true : false : true; // If there is no dropdown then the default value is TRUE

    protected override bool ValidateInput(ValidationType validationType)
    {
        bool validated = true;

        if (validationType == ValidationType.Create)
        {
            if (string.IsNullOrWhiteSpace(RoomNameInput))
            {
                Debug.LogWarning("Room name is empty");
                validated = false;
            }
        }

        return base.ValidateInput(validationType) && validated;
    }

    public override void OnClick_CreateGame()
    {
        if (!ValidateInput(ValidationType.Create))
            return;

        RoomOptions roomOptions = new RoomOptions()
        {
            CleanupCacheOnLeave = false,
            IsVisible = RoomVisibleInput,            
            MaxPlayers = MaxPlayerCountInput,
            IsOpen = true,
            CustomRoomProperties = new ExitGames.Client.Photon.Hashtable()
            {
                { "seed", SeedInput },
                { "saveName", SaveNameInput}
            }
        };

        FindObjectOfType<MainMenu>().CreateGame(RoomNameInput, roomOptions, false);
    }

    public override void OnClick_LoadGame()
    {
        if (!ValidateInput(ValidationType.Load))
            return;

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
                MaxPlayers = MaxPlayerCountInput,
                IsOpen = true,
                CustomRoomProperties = new ExitGames.Client.Photon.Hashtable()
                {
                    { "seed", Convert.ToInt32(saveFileBrowser.SelectedSave.Seed) },
                    { "saveName", saveFileBrowser.SelectedSave.Name }
                }
            };

            FindObjectOfType<MainMenu>().CreateGame(!string.IsNullOrEmpty(roomNameText?.text) ? roomNameText.text : saveFileBrowser.SelectedSave.Name, roomOptions, false);
        }
    }
}
