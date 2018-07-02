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

        PlayerNetwork.LocalPlayer.GetComponent<Inventory>().RemoveItemById("pickupitem_seeds_small", 1);
        photonView.RPC(nameof(RPC_StartFarm), PhotonTargets.AllBuffered);
    }

    public void StartProcess()
    {
        timeLeft = timeToGrow;
        isGrowing = true;
        itemOnFarm = Instantiate(melonPrefab, transform);
        itemOnFarm.transform.SetPositionAndRotation(new Vector3(transform.position.x - 0.5f, transform.position.y, transform.position.z - 0.5f), transform.rotation);
        //itemOnFarm = PhotonNetwork.Instantiate("Melon", new Vector3(transform.position.x - 0.5f, transform.position.y, transform.position.z - 0.5f), transform.rotation, 0);
        itemOnFarm.GetComponent<ResourceOnFarm>().Initialize(this);
        StartCoroutine(Grow());

        //photonView.RPC(nameof(RPC_SetItemOnFarmByViewID), PhotonTargets.AllBuffered, itemOnFarm.GetComponent<PhotonView>().viewID);
        //photonView.RPC(nameof(RPC_Grow), PhotonTargets.AllBuffered);
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
    protected void RPC_StartFarm()
    {
        StartProcess();
    }
    
}
