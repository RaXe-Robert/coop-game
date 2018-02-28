using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerNetwork : MonoBehaviour
{
    public string PlayerName { get; private set; }

    private void Awake()
    {
        PlayerName = "User" + Random.Range(1000, 9999);

        SceneManager.sceneLoaded += OnSceneFinishedLoading;
    }

    private void OnSceneFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        //TODO: HARDCODED
        if (scene.name == "Game")
        {
            Vector3 position = new Vector3();
            position.x = 0;
            position.y = 0.5f;
            position.z = 0;
            PhotonNetwork.Instantiate("Player", position, Quaternion.identity, 0);
        }
    }
}
