using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class PlayerNetwork : Photon.MonoBehaviour
{
    public static string PlayerName
    {
        get { return PhotonNetwork.player.NickName; }
        set { PhotonNetwork.player.NickName = value; }
    }
    public static List<GameObject> AllPlayers { get; private set; }

    private void OnPhotonPlayerConnected(NetworkPlayer player)
    {
        FindAllPlayers();
    }

    private void OnPhotonPlayerDisconnected(NetworkPlayer player)
    {
        FindAllPlayers();
    }

    private void FindAllPlayers()
    {
        AllPlayers.Clear();
        foreach (var item in FindObjectsOfType<PlayerNameTag>())
        {
            AllPlayers.Add(item.gameObject);
        }
    }

    public static GameObject PlayerObject { get; private set; } = null;

    private void Awake()
    {  
        SceneManager.sceneLoaded += OnSceneFinishedLoading;
        AllPlayers = new List<GameObject>();
    }

    private void OnSceneFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        //TODO: HARDCODED
        if (scene.name == "Game")
        {
            Vector3 position = new Vector3(Random.Range(-5f, 5f), 0.2f, Random.Range(0.5f, 5f));
            PlayerObject = PhotonNetwork.Instantiate("Player", position, Quaternion.identity, 0);
            FindAllPlayers();
        }
        else if (scene.name == "MainMenu")
        {
            Debug.Log("Returned to main menu");
        }
    }
}
