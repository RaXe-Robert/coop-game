using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Farm : BuildableWorldObject
{
    [SerializeField] private GameObject melonPrefab;
    public bool isGrowing = false;

    private float timeToGrow = 1F; //should be 600 , 10 min. //set to 1 for testing.
    public float timeLeft;

    public GameObject itemOnFarm = null;

    protected override void Start()
    {
        Actions = new List<UnityAction>();
        Actions.AddRange(InitializeActions());
    }

    protected override UnityAction[] InitializeActions()
    {
        return new UnityAction[]
        {
            new UnityAction(DestroyFarm),
            new UnityAction(PlaceSeed)
        };
    }

    public override void Interact(GameObject invoker, Item item)
    {
        if (!InRange(invoker.transform.position))
            return;

        if (item?.GetType() == typeof(Tool))
        {
            Tool tool = item as Tool;
            if (tool.ToolType == ToolType.Hammer)
                Actions[0].Invoke();
        }
        else if (item?.Id == "pickupitem_seeds_small")
            Actions[1].Invoke();
        else if (item != null)
            FeedUI.Instance.AddFeedItem("You can't farm " + item.Name + ", you silly.", feedType: FeedItem.Type.Fail);
        else
            FeedUI.Instance.AddFeedItem("Try to find some seeds", feedType: FeedItem.Type.World);
    }

    private void DestroyFarm()
    {
        Debug.Log(itemOnFarm);
    }

    private void PlaceSeed()
    {
        if (itemOnFarm)
        {
            FeedUI.Instance.AddFeedItem("Something is already growing!", feedType: FeedItem.Type.Fail);
            return;
        }
                
        Initialize();
    }

    public void Initialize()
    {
        timeLeft = timeToGrow;
        isGrowing = true;

        TakeSeed();
        SpawnMelon();
    }

    public void TakeSeed()
    {
        PlayerNetwork.LocalPlayer.GetComponent<Inventory>().RemoveItemById("pickupitem_seeds_small", 1);
    }

    public void SpawnMelon()
    {
        //the object data array works locally but not in multiplayer, need a way to fix this.
        itemOnFarm = PhotonNetwork.Instantiate("Melon", new Vector3(transform.position.x - 0.5f, transform.position.y, transform.position.z - 0.5f), transform.rotation, 0, new object[] { this });
        
        StartCoroutine(Grow());
    }

    IEnumerator Grow()
    {
        while (isGrowing)
        {
            yield return new WaitForSeconds(timeToGrow / timeToGrow);
            timeLeft -= timeToGrow / timeToGrow;
            
            if(timeLeft < 1)
            {
                isGrowing = false;
            }
        }
    }
}
