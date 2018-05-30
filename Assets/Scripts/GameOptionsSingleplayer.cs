using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Configurable game options for the user to create a game room.
/// </summary>
public class GameOptionsSingleplayer : MonoBehaviour
{
    protected enum ValidationType { Create, Load }

    [Header("General settings")]
    [SerializeField] private Text saveNameText;
    protected string SaveNameInput => saveNameText != null ? saveNameText.text : string.Empty;

    [SerializeField] private Text seedText;
    protected int SeedInput => string.IsNullOrWhiteSpace(seedText?.text) ? UnityEngine.Random.Range(int.MinValue, int.MaxValue) : Convert.ToInt32(seedText.text);
    
    protected virtual bool ValidateInput(ValidationType validationType)
    {
        if (validationType == ValidationType.Create)
        {
            if (string.IsNullOrWhiteSpace(SaveNameInput))
            {
                Debug.LogWarning("Invalid player input, save name is empty!");
                return false;
            }
            if (!SaveDataManager.Instance.ValidateNewSaveName(SaveNameInput))
            {
                Debug.LogWarning("Invalid player input, a save file with the same name already exists!");
                return false;
            }
        }

        return true;
    }

    public virtual void OnClick_CreateGame()
    {
        if (!ValidateInput(ValidationType.Create))
            return;

        RoomOptions roomOptions = new RoomOptions()
        {
            CleanupCacheOnLeave = false,
            IsOpen = false,
            CustomRoomProperties = new ExitGames.Client.Photon.Hashtable()
            {
                { "seed", SeedInput },
                { "saveName", SaveNameInput}
            }
        };

        FindObjectOfType<MainMenu>().CreateGame("_OfflineInstance", roomOptions, true);
    }

    public virtual void OnClick_LoadGame()
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
