using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Farm : BuildableWorldObject
{
    [SerializeField] private GameObject melonPrefab;

    private float timeToGrow = 1; //should be 600 , 10 min. //set to 1 for testing.

    public bool isGrowing { get; private set; } = false;
    public float TimeLeft { get; private set; }

    private GameObject resourceOnFarm = null;

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

        if (resourceOnFarm != null)
        {
            if (!isGrowing)
                photonView.RPC(nameof(RPC_FarmResource), PhotonTargets.AllBuffered);
            else
                FeedUI.Instance.AddFeedItem("Take your time:" + TimeLeft + " s", feedType: FeedItem.Type.Fail);
        }
        else
        {
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
    }

    private void DestroyFarm()
    {
        Debug.Log(resourceOnFarm);
    }

    private void PlaceSeed()
    {
        if (resourceOnFarm)
        {
            FeedUI.Instance.AddFeedItem("Something is already growing!", feedType: FeedItem.Type.Fail);
            return;
        }

        TimeLeft = timeToGrow;
        isGrowing = true;

        photonView.RPC(nameof(RPC_SpawnResource), PhotonTargets.AllBuffered);
    }

    [PunRPC]
    public void RPC_SpawnResource()
    {
        PlayerNetwork.LocalPlayer.GetComponent<Inventory>().RemoveItemById("pickupitem_seeds_small", 1);
        
        resourceOnFarm = Instantiate(melonPrefab, new Vector3(transform.position.x - 0.5f, transform.position.y, transform.position.z - 0.5f), transform.rotation);
        
        StartCoroutine(Grow());
    }

    [PunRPC]
    public void RPC_FarmResource()
    {
        PlayerNetwork.LocalPlayer.GetComponent<Inventory>().AddItemById("pickupitem_melon", 1);

        FeedUI.Instance.AddFeedItem("Farm harvested", feedType: FeedItem.Type.World);
        
        Destroy(resourceOnFarm);
        resourceOnFarm = null;
    }

    IEnumerator Grow()
    {
        while (isGrowing)
        {
            yield return new WaitForSeconds(timeToGrow / timeToGrow);
            TimeLeft -= timeToGrow / timeToGrow;
            
            if(TimeLeft < 1)
            {
                isGrowing = false;
            }
        }
    }
}
