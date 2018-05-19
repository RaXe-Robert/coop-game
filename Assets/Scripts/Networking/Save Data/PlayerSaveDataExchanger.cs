using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;

[RequireComponent(typeof(PhotonView))]
public class PlayerSaveDataExchanger : Photon.PunBehaviour
{
    public static PlayerSaveDataExchanger Instance { get; private set; }

    private static List<PhotonPlayer> otherPlayersToLoad = new List<PhotonPlayer>();

    private string persistentDataPath;
    public string PersistentDataPath
    {
        get
        {
#if UNITY_EDITOR
            return $"{persistentDataPath}/Editor/Saves/";
#else
            return $"{persistentDataPath}/Saves/";
#endif
        }
        private set { persistentDataPath = value; }
    }
    public string WorldDataPath => $"{PersistentDataPath}World/";
    public string PlayerInfoDataPath => $"{PersistentDataPath}Players/";

    /// <summary>
    /// Clears all event handlers after the invoke has taken place.
    /// </summary>
    public event Action<bool> OnWorldDownloaded;
    public bool IsWorldDownloaded { get; private set; }
    private void SetWorldDownloaded(bool state)
    {
        IsWorldDownloaded = state;

        if (state)
        {
            OnWorldDownloaded?.Invoke(true);
            OnWorldDownloaded = null;
        }
    }

    private void Awake()
    {
        Instance = FindObjectOfType<PlayerSaveDataExchanger>();

        CreateDataPaths();
    }

    private void CreateDataPaths()
    {
        persistentDataPath = Application.persistentDataPath;

        if (!Directory.Exists(WorldDataPath))
            Directory.CreateDirectory(WorldDataPath);
        if (!Directory.Exists(PlayerInfoDataPath))
            Directory.CreateDirectory(PlayerInfoDataPath);
    }

    private void OnEnable()
    {
        PhotonNetwork.OnEventCall += this.OnManifestUpdateEvent;
    }

    private void OnDisable()
    {
        PhotonNetwork.OnEventCall -= this.OnManifestUpdateEvent;
    }

    public void UpdateManifest()
    {
        PhotonNetwork.RaiseEvent(0, new object[0], true, null);
    }

    private void OnManifestUpdateEvent(byte eventcode, object content, int senderid)
    {
        if (eventcode == 0)
        {
            Debug.Log("Event with code: '0'. Updating manifest of other player");

            PhotonPlayer sender = PhotonPlayer.Find(senderid);
            byte[] selected = content as byte[];
            /*
            for (int i = 0; i < selected.Length; i++)
            {
                byte unitId = selected[i];
            }*/
        }
    }


    private IEnumerator ProcessPlayersToLoad()
    {
        for (int i = otherPlayersToLoad.Count - 1; i >= 0; i--)
        {
            List<Tuple<string, byte[]>> files = LoadWorldFiles();

            if (files.Count == 0)
                photonView.RPC(nameof(SendWorldData), PhotonTargets.Others, "empty", null, 0);
            else
            {
                for (int y = 0; y < files.Count; y++)
                {
                    photonView.RPC(nameof(SendWorldData), PhotonTargets.Others, files[y].Item1, files[y].Item2, files.Count - y - 1);
                    yield return new WaitForSecondsRealtime(0.02f);
                }
            }

            otherPlayersToLoad.RemoveAt(i);
        }
    }

    [PunRPC]
    private void SendWorldData(string fileName, byte[] fileRaw, int filesRemaining)
    {
        if (IsWorldDownloaded)
            return;

        if (fileRaw != null && fileRaw.Length > 0)
        {
            Debug.Log($"Saving: {WorldDataPath + fileName}");

            if (!Directory.Exists(WorldDataPath))
                Directory.CreateDirectory(WorldDataPath);

            try
            {
                using (var fs = new FileStream(WorldDataPath + fileName, FileMode.Create, FileAccess.Write))
                {
                    fs.Write(fileRaw, 0, fileRaw.Length);
                }
            }
            catch (IOException e)
            {
                Debug.LogError(e);
            }
        }

        if (filesRemaining == 0)
        {
            SetWorldDownloaded(true);
        }
    }

    private List<Tuple<string, byte[]>> LoadWorldFiles()
    {
        string directoryPath = WorldDataPath;

        string[] files = Directory.GetFiles(directoryPath);

        List<Tuple<string, byte[]>> filesRaw = new List<Tuple<string, byte[]>>();

        foreach (var file in files)
        {
            FileStream fileStream = File.Open(file, FileMode.Open);
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    fileStream.CopyTo(ms);
                    Tuple<string, byte[]> dataTuple = Tuple.Create(Path.GetFileName(file), ms.ToArray());
                    filesRaw.Add(dataTuple);
                }

            }
            catch (IOException e)
            {
                Debug.LogError(e);
            }
            finally
            {
                fileStream.Close();
            }
        }

        return filesRaw;
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
        if (PhotonNetwork.isMasterClient)
            SetWorldDownloaded(true);
        else
            SetWorldDownloaded(false);
    }

    public override void OnJoinedLobby()
    {
        SetWorldDownloaded(false);
    }

    #endregion //Photon Callbacks
}
