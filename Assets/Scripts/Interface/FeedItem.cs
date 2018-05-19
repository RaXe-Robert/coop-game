using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeedItem : MonoBehaviour {

    private float WaitTime = 2;
    private float FadeScale = 0.1F;
    private CanvasGroup CanvasGroup;
    private bool Fading = false;
    
    private void Start () {
        CanvasGroup = GetComponent<CanvasGroup>();
        Debug.Log("Timer??");
        StartCoroutine(Timer());
    }

    public void Delete()
    {
        Fading = true;
        StartCoroutine(FadeItem());
    }

    /// <summary>
    /// This starts fading the item when its not already doing this.
    /// </summary>
    IEnumerator Timer()
    {
        while (Fading == false)
        {
            yield return new WaitForSeconds(1F);
            WaitTime -= 1F;
            if (WaitTime <= 0)
            {
                Fading = true;
                StartCoroutine(FadeItem());
            }
        }
    }

    /// <summary>
    /// This Fades the item when its not already fading.
    /// </summary>
    IEnumerator FadeItem()
    {
        while (Fading == true)
        {
            yield return new WaitForSeconds(0.1F);
            FadeOutItem();
        }
    }

    private void FadeOutItem()
    {
        if (CanvasGroup.alpha != 0)
            CanvasGroup.alpha -= FadeScale;
        else
        {
            FeedUI.Instance.RemoveFromCurrentItems(gameObject);
            Destroy(gameObject);
        }
    }
}
