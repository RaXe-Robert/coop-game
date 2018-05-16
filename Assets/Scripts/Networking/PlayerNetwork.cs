using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

using UnityEngine;
using UnityEngine.SceneManagement;

using Assets.Scripts.Map_Generation;
using static Assets.Scripts.Map_Generation.DataMapGenerator;
using System;

[RequireComponent(typeof(PhotonView))]
public class PlayerNetwork : Photon.PunBehaviour
{
    public static string PlayerName
    {
        get { return PhotonNetwork.player.NickName; }
        set { PhotonNetwork.player.NickName = value; }
    }

    public static GameObject PlayerObject { get; private set; } = null;

    public static List<PhotonPlayer> OtherPlayers = new List<PhotonPlayer>();
    private static List<PhotonPlayer> otherPlayersToLoad = new List<PhotonPlayer>();

    /// <summary>
    /// Clears all event handlers after the invoke has taken place.
    /// </summary>
    public static event Action<bool> OnWorldDownloaded;
    public static bool IsWorldDownloaded { get; private set; } = false;

    private void Awake()
    {  
        SceneManager.sceneLoaded += OnSceneFinishedLoading;
    }

    private void OnSceneFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        //TODO: HARDCODED
        if (scene.name == "Game")
        {
            Vector3 position = new Vector3(UnityEngine.Random.Range(-5f, 5f), 10f, UnityEngine.Random.Range(0.5f, 5f));
            PlayerObject = PhotonNetwork.Instantiate("Player", position, Quaternion.identity, 0);

            // The owner already has access to the save files
            if (PhotonNetwork.isMasterClient)
                IsWorldDownloaded = true;
            else
                IsWorldDownloaded = false;

            StartCoroutine(ProcessPlayersToLoad());
        }
        else if (scene.name == "MainMenu")
        {
            Debug.Log("Returned to main menu");
        }
    }

    public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
    {
        OtherPlayers.Add(newPlayer);
        otherPlayersToLoad.Add(newPlayer);
        
        if (SceneManager.GetActiveScene().name == "Game")
        {
            StartCoroutine(ProcessPlayersToLoad());
        }
    }

    public override void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
    {
        OtherPlayers.Remove(otherPlayer);
    }
    
    private IEnumerator ProcessPlayersToLoad()
    {
        for (int i = otherPlayersToLoad.Count - 1; i >= 0; i--)
        {
            List<Tuple<string, byte[]>> files = LoadFiles();

            for (int y = 0; y < files.Count; y++)
            {
                photonView.RPC(nameof(SendMapData), PhotonTargets.Others, files[y].Item1, files[y].Item2, files.Count - y - 1);
                yield return new WaitForSeconds(0.1f);
            }

            otherPlayersToLoad.RemoveAt(i);
        }
    }
    
    [PunRPC]
    private void SendMapData(string fileName, byte[] fileRaw, int filesRemaining)
    {
        Debug.Log($"Saving: {TerrainGenerator.WorldDataPath + fileName}");

        if (!Directory.Exists(TerrainGenerator.WorldDataPath))
            Directory.CreateDirectory(TerrainGenerator.WorldDataPath);
        
        try
        {
            using (var fs = new FileStream(TerrainGenerator.WorldDataPath + fileName, FileMode.Create, FileAccess.Write))
            {
                fs.Write(fileRaw, 0, fileRaw.Length);
            }
        }
        catch (IOException e)
        {
            Debug.LogError(e);
        }

        if (filesRemaining == 0)
        {
            IsWorldDownloaded = true;

            OnWorldDownloaded?.Invoke(true);
            OnWorldDownloaded = null;
        }
    }

    private List<Tuple<string, byte[]>> LoadFiles()
    {
        string directoryPath = string.Empty;
#if UNITY_EDITOR
        directoryPath = "C:/Users/Robert/AppData/LocalLow/Groep6/JustSurvive/349260201/Editor";
#else
        directoryPath = "C:/Users/Robert/AppData/LocalLow/Groep6/JustSurvive/349260201";
#endif
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
}
