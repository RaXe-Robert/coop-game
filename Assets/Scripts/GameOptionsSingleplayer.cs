using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Configurable game options for the user to create a game room.
/// </summary>
public class GameOptionsSingleplayer : MonoBehaviour
{
    [Header("General settings")]
    [SerializeField] protected Text saveNameText;
    [SerializeField] protected Text seedText;
    
    public virtual void OnClick_CreateGame()
    {
        RoomOptions roomOptions = new RoomOptions()
        {
            CleanupCacheOnLeave = false,
            IsOpen = false,
            CustomRoomProperties = new ExitGames.Client.Photon.Hashtable()
            {
                { "seed", Convert.ToInt32(seedText.text) },
                { "saveName", saveNameText.text}
            }
        };

        FindObjectOfType<MainMenu>().CreateGame("_OfflineInstance", roomOptions, true);
    }

    public virtual void OnClick_LoadGame()
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
                IsOpen = false,
                MaxPlayers = 1,
                IsVisible = false,
                CleanupCacheOnLeave = true,
                CustomRoomProperties = new ExitGames.Client.Photon.Hashtable()
                {
                    { "seed", Convert.ToInt32(saveFileBrowser.SelectedSave.Seed) },
                    { "saveName", saveFileBrowser.SelectedSave.Name }
                }
            };

            FindObjectOfType<MainMenu>().CreateGame("_OfflineInstance", roomOptions, true);
        }
    }
}
