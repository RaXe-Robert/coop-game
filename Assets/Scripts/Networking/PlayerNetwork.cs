using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerNetwork : MonoBehaviour
{
    public static string PlayerName
    {
        get
        {
            return PhotonNetwork.player.NickName;
        }
        set
        {
            PhotonNetwork.player.NickName = value;
        }
    }

    public GameObject PlayerObject { get; private set; }


    private void Awake()
    {  
        SceneManager.sceneLoaded += OnSceneFinishedLoading;
    }

    private void OnSceneFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        //TODO: HARDCODED
        if (scene.name == "Game")
        {
            PlayerObject = PhotonNetwork.Instantiate("Player", Vector3.up, Quaternion.identity, 0);
        }
        else if (scene.name == "MainMenu")
        {
            Debug.Log("Returned to main menu");
        }
    }
}
