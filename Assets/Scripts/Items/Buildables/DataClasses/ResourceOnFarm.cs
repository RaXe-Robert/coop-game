using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceOnFarm : Photon.MonoBehaviour, IInteractable {

    Farm farm;
    public float InteractDistance => farm.interactDistance;

    public void Initialize(Farm f)
    {
        farm = f;
    }

    public bool IsInteractable => true;
    public GameObject GameObject => farm.itemOnFarm;

    public string TooltipText => $"Some juicy melons for you and your companions";

    public bool InRange(Vector3 invokerPosition) => Vector3.Distance(invokerPosition, transform.position) < InteractDistance;

    public void Interact(GameObject invoker, Item item)
    {
        if (!InRange(invoker.transform.position))
            return;

        if (!farm.isGrowing)
            FarmResource();
        else
            FeedUI.Instance.AddFeedItem("Take your time:" + farm.timeLeft + " s", feedType: FeedItem.Type.Fail);
    }

    public void FarmResource()
    {
        PlayerNetwork.LocalPlayer.GetComponent<Inventory>().AddItemById("pickupitem_melon", PlayerNetwork.OtherPlayers.Count + 1);      
        FeedUI.Instance.AddFeedItem("Melon farmed", feedType: FeedItem.Type.World);
        photonView.RPC(nameof(RPC_DestroyFarmObject), PhotonTargets.AllBuffered);
    }

    [PunRPC]
    protected void RPC_DestroyFarmObject()
    {
        Destroy(farm.itemOnFarm);        
    }
}
