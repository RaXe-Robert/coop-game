using UnityEngine;
using Photon;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class PlayerNetwork : Photon.MonoBehaviour
{
    public static string PlayerName
    {
        get { return PhotonNetwork.player.NickName; }
        set { PhotonNetwork.player.NickName = value; }
    }

    public static GameObject LocalPlayer { get; private set; } = null;

    private void Awake()
    {  
        SceneManager.sceneLoaded += OnSceneFinishedLoading;
    }

    private void OnSceneFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        //TODO: HARDCODED
        if (scene.name == "Game")
        {
            SpawnPlayer();
        }
        else if (scene.name == "MainMenu")
        {
            Debug.Log("Returned to main menu");
        }
    }

    public void SpawnPlayer()
    {
        Vector3 position = new Vector3(Random.Range(-5f, 5f), 0.2f, Random.Range(0.5f, 5f));
        LocalPlayer = PhotonNetwork.Instantiate("Player", position, Quaternion.identity, 0);
    }
}
