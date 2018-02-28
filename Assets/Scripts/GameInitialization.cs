using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameInitialization : MonoBehaviour {

    // Use this for initialization
    void Start()
    {
        StartCoroutine(DelayedMainMenuLoad(1.5f));
    }

    private IEnumerator DelayedMainMenuLoad(float seconds)
    {
        yield return new WaitForSecondsRealtime(seconds);
        SceneManager.LoadScene("MainMenu");
    }
}
