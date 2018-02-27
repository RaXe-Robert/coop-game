using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenuScript : MonoBehaviour {

    [SerializeField] private GameObject escapeMenu;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown("escape"))
        {
            escapeMenu.SetActive(!escapeMenu.GetActive());
        }
	}

    public void ResumeGame()
    {
        escapeMenu.SetActive(false);
    }

    public void QuitGame()
    {
        PhotonNetwork.LeaveRoom();       
        SceneManager.LoadScene("MainMenu");
    }
}
