using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeedItem : MonoBehaviour {

    /// <summary>
    /// This specifies the type of the message, did the user succeed on something? Or did something went wrong?
    /// In FeedUI this enum decides which color the message gets.
    /// </summary>
    public enum Type
    {
        None,
        Default,
        Succes,
        Fail,
        Error,
        World        
    }

    private float WaitTime = 2;
    private float FadeScale = 0.1F;
    private CanvasGroup CanvasGroup;
    private RectTransform ItemRectTransform;
    private bool Fading = false;
    
    private void Start () {
        CanvasGroup = GetComponent<CanvasGroup>();
        ItemRectTransform = GetComponent<RectTransform>();
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
            transform.localScale = new Vector3(1, transform.localScale.y - FadeScale, 1);
            ItemRectTransform.sizeDelta = new Vector2(ItemRectTransform.sizeDelta.x, ItemRectTransform.sizeDelta.y * transform.localScale.y);
        }
        else
        {
            FeedUI.Instance.RemoveFromCurrentItems(gameObject);
            Destroy(gameObject);
        }
    }
}
