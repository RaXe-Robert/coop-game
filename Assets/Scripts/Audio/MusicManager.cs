using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour {

    [SerializeField]
    private AudioClip[] mainMenuSceneClips;
    [SerializeField]
    private AudioClip[] gameSceneClips;

    private AudioSource audioSource;

    private bool sceneChanged = false;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        StartCoroutine(PlayMusic());
    }

    private void OnEnable() => SceneManager.sceneLoaded += OnSceneChanged;
    private void OnDisable() => SceneManager.sceneLoaded -= OnSceneChanged;

    private void OnSceneChanged(Scene scene, LoadSceneMode loadSceneMode)
    {
        sceneChanged = true;
    }

    private IEnumerator PlayMusic()
    {
        while (true)
        {
            string sceneName = SceneManager.GetActiveScene().name;

            int randClip = 0;

            switch (sceneName)
            {
                case "MainMenu":
                    randClip = Random.Range(0, mainMenuSceneClips.Length);
                    audioSource.clip = mainMenuSceneClips[randClip];
                    break;
                case "Game":
                    randClip = Random.Range(0, gameSceneClips.Length);
                    audioSource.clip = gameSceneClips[randClip];
                    break;
            }

            audioSource.Play();

            yield return new WaitWhile(() => audioSource.isPlaying && !sceneChanged);

            sceneChanged = false;
        }
    }
}
