using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeedItem : MonoBehaviour {

    public enum FeedColor
    {
        None,
        Succes,
        Fail,
        Error,
        World
    }

    private float WaitTime = 2;
    private float FadeScale = 0.1F;
    private CanvasGroup CanvasGroup;
    private RectTransform RectTransform;
    private bool Fading = false;
    
    private void Start () {
        CanvasGroup = GetComponent<CanvasGroup>();
        RectTransform = GetComponent<RectTransform>();
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
        {
            CanvasGroup.alpha -= FadeScale;
            RectTransform.sizeDelta = new Vector2(RectTransform.sizeDelta.x, RectTransform.sizeDelta.y - FadeScale * RectTransform.sizeDelta.y);
        }
        else
        {
            FeedUI.Instance.RemoveFromCurrentItems(gameObject);
            Destroy(gameObject);
        }
    }
}
