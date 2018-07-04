using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Farm : BuildableWorldObject
{
    [SerializeField] private GameObject melonPrefab;
    [SerializeField] private GameObject ruinedFarmPrefab;
    public bool isGrowing = false;

    private float timeToGrow = 600F;
    public float timeLeft;

    public GameObject resourceOnFarm = null;

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
        else if (resourceOnFarm)
            FeedUI.Instance.AddFeedItem("Something is already growing!", feedType: FeedItem.Type.Fail);
        else
        {
            if (item != null)
                FeedUI.Instance.AddFeedItem("You can't farm " + item.Name + ", you silly.", feedType: FeedItem.Type.Fail);
            else
                FeedUI.Instance.AddFeedItem("Try to find some seeds", feedType: FeedItem.Type.World);
        }
    }

    private void DestroyFarm()
    {
        photonView.RPC(nameof(RPC_DestroyFarm), PhotonTargets.AllBuffered);
    }

    private void PlaceSeed()
    {
        if (resourceOnFarm)
        {
            FeedUI.Instance.AddFeedItem("Something is already growing!", feedType: FeedItem.Type.Fail);
            return;
        }
                
        Initialize();
        FeedUI.Instance.AddFeedItem("Seed planted!", feedType: FeedItem.Type.World);
    }

    public void Initialize()
    {
        timeLeft = timeToGrow;
        isGrowing = true;

        TakeSeed();

        photonView.RPC(nameof(RPC_SpawnResource), PhotonTargets.AllBuffered);
    }

    public void TakeSeed()
    {
        PlayerNetwork.LocalPlayer.GetComponent<Inventory>().RemoveItemById("pickupitem_seeds_small", 1);
    }

    public void GiveMelon()
    {
        PlayerNetwork.LocalPlayer.GetComponent<Inventory>().AddItemById("pickupitem_melon", PlayerNetwork.OtherPlayers.Count + 1);
    }

    public void Harvest()
    {
        photonView.RPC(nameof(RPC_FarmResource), PhotonTargets.AllBuffered);
        GiveMelon();
        FeedUI.Instance.AddFeedItem("Farm emptied", feedType: FeedItem.Type.World);
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

    [PunRPC]
    protected void RPC_SpawnResource()
    {
        resourceOnFarm = Instantiate(melonPrefab, new Vector3(transform.position.x - 0.5f, transform.position.y, transform.position.z - 0.5f), transform.rotation);
        resourceOnFarm.GetComponent<ResourceOnFarm>().Initialize(this);
        StartCoroutine(Grow());
    }

    [PunRPC]
    protected void RPC_FarmResource()
    {
        Destroy(resourceOnFarm);
        resourceOnFarm = null;
    }

    [PunRPC]
    protected void RPC_DestroyFarm()
    {
        Instantiate(ruinedFarmPrefab, transform.position, transform.rotation);
        Destroy(resourceOnFarm);
        Destroy(gameObject);
    }
}
