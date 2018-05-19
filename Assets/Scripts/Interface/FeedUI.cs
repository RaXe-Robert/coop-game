using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FeedUI : MonoBehaviour {

    public GameObject feedItemPrefab;
    public GameObject feed;

    public static FeedUI Instance { get; private set; }

    private List<GameObject> currentItems;
    private int preferredAmount = 4;

    public void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    private void Start()
    {
        currentItems = new List<GameObject>();
    }

    public void AddFeedItem(Sprite sprite, string message)
    {
        GameObject go = Instantiate(feedItemPrefab);
        go.transform.SetParent(feed.transform);
        go.transform.localScale = Vector3.one;

        Text FeedText = go.transform.Find("FeedText").GetComponent<Text>();
        FeedText.text = message;

        Image FeedImage = go.transform.Find("FeedImage").GetComponent<Image>();
        FeedImage.sprite = sprite;

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
