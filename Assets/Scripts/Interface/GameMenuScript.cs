using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenuScript : MonoBehaviour {

    [SerializeField] private GameObject escapeMenu;
    
	// Update is called once per frame
	private void Update () {
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
        Debug.Log("Room cleanup... leaving");
        PhotonNetwork.DestroyPlayerObjects(PhotonNetwork.player.ID);
        PhotonNetwork.LeaveRoom();       
    }

    #region Photon Callbacks

    private void OnLeftRoom()
    {
        Debug.Log("Succesfully left the room");
        PhotonNetwork.LoadLevel("MainMenu");
    }

    #endregion //Photon Callbacks

}
