using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour {

    [SerializeField]
    private AudioClip[] mainMenuSceneClips;
    [SerializeField]
    private AudioClip[] gameSceneClips;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable() => SceneManager.sceneLoaded += PlayMusic;
    private void OnDisable() => SceneManager.sceneLoaded -= PlayMusic;

    private void PlayMusic(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (scene.name == "MainMenu")
        {
            int randClip = Random.Range(0, mainMenuSceneClips.Length);
            audioSource.clip = mainMenuSceneClips[randClip];
        }
        else if (scene.name == "Game")
        {
            int randClip = Random.Range(0, gameSceneClips.Length);
            audioSource.clip = gameSceneClips[randClip];
        }

        audioSource.Play();
    }
}
