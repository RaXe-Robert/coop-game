using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameInitialization : MonoBehaviour {

    // Use this for initialization
    void Start()
    {
#if UNITY_EDITOR
        StartCoroutine(DelayedMainMenuLoad(0f));
#else
        StartCoroutine(DelayedMainMenuLoad(1.5f));
#endif
    }

    private IEnumerator DelayedMainMenuLoad(float seconds)
    {
        yield return new WaitForSecondsRealtime(seconds);
        SceneManager.LoadScene("MainMenu");
    }
}
