using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FeedUI : MonoBehaviour {

    public GameObject feedItemPrefab;
    public GameObject feed;

    public static FeedUI Instance { get; private set; }

    private List<GameObject> currentItems;
    private int preferredAmount = 5;

    public void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    private void Start()
    {
        currentItems = new List<GameObject>();
    }

    /// <summary>
    /// This method spawns a ui item in the feed with given Sprite, string and FeedItem.FeedType. The feedtype will decide what color the message gets.
    /// When the feedType is not specified the message will be grey.
    /// </summary>
    /// <param name="sprite"></param>
    /// <param name="message"></param>
    /// <param name="feedType"></param>
    public void AddFeedItem(string message, Sprite sprite = null, FeedItem.Type feedType = FeedItem.Type.None)
    {
        GameObject go = Instantiate(feedItemPrefab);
        go.transform.SetParent(feed.transform);
        go.transform.localScale = Vector3.one;

        Text FeedText = go.transform.Find("FeedText").GetComponent<Text>();
        FeedText.text = message;

        Image FeedImage = go.transform.Find("FeedImage").GetComponent<Image>();

        if (sprite != null)
            FeedImage.sprite = sprite;
        else
            FeedImage.color = new Color32(0, 0, 0, 0);

        Image BackgroundImage = go.transform.GetComponent<Image>();

        switch (feedType)
        {
            case FeedItem.Type.None:
                BackgroundImage.color = new Color32(0, 0, 0, 0);  //0 Alpha
                break;
            case FeedItem.Type.Default:
                BackgroundImage.color = new Color32(0, 0, 0, 50);  //Grey
                break;
            case FeedItem.Type.Succes:
                BackgroundImage.color = new Color32(0, 128, 16, 50);  //Green
                break;
            case FeedItem.Type.Fail:
                BackgroundImage.color = new Color32(191, 0, 6, 50);  //Red
                break;
            case FeedItem.Type.Error:
                BackgroundImage.color = new Color32(191, 0, 6, 100);  //Red but more alpha
                break;
            case FeedItem.Type.World:
                BackgroundImage.color = new Color32(0, 65, 191, 50);  //Blue
                break;
        }

        currentItems.Add(go);
        if(currentItems.Count > preferredAmount)
        {
            currentItems[0].GetComponent<FeedItem>().Delete();
            currentItems.RemoveAt(0);
        }
    }

    public void RemoveFromCurrentItems(GameObject go)
    {
        currentItems.Remove(go);
    }
}
