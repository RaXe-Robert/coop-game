﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

    public GameObject optionsPanel;

    private void Start()
    {
        //Initialize volume to 50% so people don't go deaf.
        AudioListener.volume = 0.5f;
    }

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

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
