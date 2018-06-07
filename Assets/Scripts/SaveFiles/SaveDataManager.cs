﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;

[RequireComponent(typeof(PhotonView))]
public class SaveDataManager : Photon.PunBehaviour
{
    public static SaveDataManager Instance { get; private set; }

    private static List<PhotonPlayer> otherPlayersToLoad = new List<PhotonPlayer>();

    /// <summary> Contains the manifests of the other players. </summary>
    private Dictionary<PhotonPlayer, byte[]> playerManifests;

    public static string PersistentDataPath { get; private set; }

    public string RoomFolderName { get; private set; }

    public static string SaveFileFolder
    {
        get
        {
#if UNITY_EDITOR
            return $"{PersistentDataPath}/Editor/Saves/";
#else
            return $"{PersistentDataPath}/Saves/";
#endif
        }
    }
    public string CurrentSaveFileDataPath
    {
        get
        {
            string roomFolder = string.IsNullOrEmpty(RoomFolderName) ? string.Empty : $"{RoomFolderName}/";
            return $"{SaveFileFolder}{roomFolder}";
        }
    }
    
    public string WorldDataPath
    {
        get
        {
            string path = $"{CurrentSaveFileDataPath}World/";

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            return path;
        }
    }

    public string PlayerDataPath
    {
        get
        {
            string path = $"{CurrentSaveFileDataPath}Players/";

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            return path;
        }
    }

    /// <summary>
    /// Clears all event handlers after the invoke has taken place.
    /// </summary>
    public event Action OnWorldDownloaded;
    public bool IsWorldDownloaded { get; private set; }
    private void SetWorldDownloaded(bool state)
    {
        IsWorldDownloaded = state;

        if (state)
        {
            OnWorldDownloaded?.Invoke();
            OnWorldDownloaded = null;

            UpdateManifest();
        }
    }

    private void Awake()
    {
        Instance = FindObjectOfType<SaveDataManager>();

        PersistentDataPath = Application.persistentDataPath;

        if (!Directory.Exists(SaveFileFolder))
            Directory.CreateDirectory(SaveFileFolder);
    }

    private void OnEnable() => PhotonNetwork.OnEventCall += this.OnManifestUpdateEvent;
    private void OnDisable() => PhotonNetwork.OnEventCall -= this.OnManifestUpdateEvent;

    public void UpdateManifest()
    {
        // Players
        PlayerSaveInfo[] playerSaveInfos = PhotonNetwork.playerList.Select(x => new PlayerSaveInfo(x.NickName, (int)(x.CustomProperties["UniqueID"] ?? -1))).ToArray();

        // Chunks
        List<Tuple<string, byte[]>> worldFiles = FileLoader.LoadFiles(WorldDataPath); // TODO: We dont want to do this every time we are saving the manifest
        ChunkSaveInfo[] chunkSaveInfos = worldFiles.Select(x => new ChunkSaveInfo(x.Item1)).ToArray();

        SaveDataManifest saveDataManifest = new SaveDataManifest
        {
            Save_TimeStamp = DaytimeController.Instance?.CurrentTime.Ticks ?? 0,

            GameTime = (long)PhotonNetwork.room.CustomProperties["gameTime"],
            Seed = (int)PhotonNetwork.room.CustomProperties["seed"],
            Name = (string)PhotonNetwork.room.CustomProperties["saveName"],

            Players = playerSaveInfos,
            Chunks = chunkSaveInfos
        };

        // Save the manifest locally
        SaveManifest(saveDataManifest);

        // Send the manifest to the other players
        PhotonNetwork.RaiseEvent(0, new byte[0], true, null);
    }

    public SaveDataManifest[] LoadAllManifests()
    {
        string[] files = Directory.GetFiles(SaveFileFolder, "*.manifest");

        List<SaveDataManifest> manifests = new List<SaveDataManifest>();

        for (int i = 0; i < files.Length; i++)
        {
            Tuple<string, byte[]> fileData = FileLoader.LoadFile(files[i]);
            if (fileData != null)
            {
                string json = System.Text.Encoding.UTF8.GetString(fileData.Item2);

                SaveDataManifest manifest = (SaveDataManifest)JsonUtility.FromJson(json, typeof(SaveDataManifest));
                if (manifest != null)
                    manifests.Add(manifest);
            }
        }
        return manifests.ToArray();
    }

    /// <summary>
    /// Validates the given save name by comparing it all other save file names.
    /// </summary>
    /// <param name="saveName">The new save file name.</param>
    /// <returns>True if no other save file has been found with the given name.</returns>
    public bool ValidateNewSaveName(string saveName)
    {
        SaveDataManifest[] manifests = LoadAllManifests();

        for (int i = 0; i < manifests.Length; i++)
        {
            if (string.Equals(manifests[i].Name, saveName))
                return false;
        }

        return true;
    }

    /// <summary>
    /// Saves the manifest to disk.
    /// </summary>
    /// <param name="saveDataManifest"></param>
    private void SaveManifest(SaveDataManifest saveDataManifest)
    {
        string json = JsonUtility.ToJson(saveDataManifest);

        WriteToFile($"{RoomFolderName}.manifest", SaveFileFolder, System.Text.Encoding.UTF8.GetBytes(json));

        Debug.Log($"Saving manifest '{saveDataManifest.Name}': {json}");
    }

    private void OnManifestUpdateEvent(byte eventcode, object content, int senderid)
    {
        if (eventcode == 0)
        {
            PhotonPlayer sender = PhotonPlayer.Find(senderid);

            if (sender == null)
            {
                Debug.LogWarning("Received manifest from sender but sender could not be find in scene.");
                return;
            }

            byte[] contentRaw = content as byte[];

            if (playerManifests.ContainsKey(sender))
                playerManifests[sender] = contentRaw;
            else
                playerManifests.Add(sender, contentRaw);
        }
    }

    /// <summary>
    /// Send save files to recently joined players.
    /// </summary>
    private IEnumerator ProcessPlayersToLoad()
    {
        for (int i = otherPlayersToLoad.Count - 1; i >= 0; i--)
        {
            // Send world files
            List<Tuple<string, byte[]>> files = FileLoader.LoadFiles(WorldDataPath);

            if (files.Count == 0)
                photonView.RPC(nameof(SendWorldData), otherPlayersToLoad[i], "empty", null, 0);
            else
            {
                for (int y = 0; y < files.Count; y++)
                {
                    photonView.RPC(nameof(SendWorldData), otherPlayersToLoad[i], files[y].Item1, files[y].Item2, files.Count - y - 1);
                    yield return new WaitForSecondsRealtime(0.02f);
                }
            }

            otherPlayersToLoad.RemoveAt(i);
        }
    }

    [PunRPC]
    private void SendWorldData(string fileName, byte[] data, int filesRemaining)
    {
        if (IsWorldDownloaded)
            return;

        if (data != null && data.Length > 0)
        {
            Debug.Log($"Saving: {WorldDataPath + fileName}");

            WriteToFile(fileName, WorldDataPath, data);
        }

        if (filesRemaining == 0)
            SetWorldDownloaded(true);
    }

    private void WriteToFile(string fileName, string path, byte[] content)
    {
        try
        {
            using (var fs = new FileStream(path + fileName, FileMode.Create, FileAccess.Write))
            {
                fs.Write(content, 0, content.Length);
            }
        }
        catch (IOException e)
        {
            Debug.LogError(e);
        }
    }

    #region Photon Callbacks

    public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
    {
        otherPlayersToLoad.Add(newPlayer);

        if (PhotonNetwork.isMasterClient && PhotonNetwork.inRoom)
        {
            StartCoroutine(ProcessPlayersToLoad());
        }
    }

    public override void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
    {
        otherPlayersToLoad.Remove(otherPlayer);
    }

    public override void OnJoinedRoom()
    {
        playerManifests = new Dictionary<PhotonPlayer, byte[]>();

        RoomFolderName = $"{(string)PhotonNetwork.room.CustomProperties["saveName"]}";

        if (PhotonNetwork.isMasterClient)
            SetWorldDownloaded(true);
        else
            SetWorldDownloaded(false);

    }

    public override void OnJoinedLobby()
    {
        playerManifests = null;

        SetWorldDownloaded(false);
    }

    #endregion //Photon Callbacks
}
