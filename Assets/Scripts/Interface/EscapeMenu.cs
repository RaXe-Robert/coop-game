using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EscapeMenu : MonoBehaviour {

    [SerializeField] private GameObject escapeMenuUI;
    
	public void ResumeGame()
    {
        escapeMenuUI.SetActive(false);
    }

    public void QuitGame()
    {
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
