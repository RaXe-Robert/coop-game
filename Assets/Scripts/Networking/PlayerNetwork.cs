using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerNetwork : MonoBehaviour
{
    public static string PlayerName
    {
        get { return PhotonNetwork.player.NickName; }
        set { PhotonNetwork.player.NickName = value; }
    }

    public static GameObject PlayerObject { get; private set; } = null;

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

    public static void SpawnPlayer()
    {
        Vector3 position = new Vector3(Random.Range(-5f, 5f), 0.2f, Random.Range(0.5f, 5f));
        PlayerObject = PhotonNetwork.Instantiate("Player", position, Quaternion.identity, 0);
    }

    public static void RespawnPlayer()
    {
        Vector3 position = new Vector3(Random.Range(-5f, 5f), 0.2f, Random.Range(0.5f, 5f));
        PlayerObject.transform.position = position;
        PlayerObject.GetComponent<HealthComponent>().SetValue(100);
        PlayerObject.GetComponent<HungerComponent>().SetValue(100);
        PlayerObject.GetComponent<PlayerCombatController>().TogglePlayerModel(true);
    }
}
