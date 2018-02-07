using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

    public GameObject optionsPanel;

    public void StartGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void ShowOptions()
    {
        optionsPanel.SetActive(true);
    }

    public void HideOptions()
    {
        optionsPanel.SetActive(false);
    }

    public void ExtiGame()
    {
        Application.Quit();
    }
}
