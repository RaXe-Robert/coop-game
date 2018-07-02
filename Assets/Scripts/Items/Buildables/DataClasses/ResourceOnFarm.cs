using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceOnFarm : Photon.MonoBehaviour, IInteractable {

    Farm farm;
    public float InteractDistance => farm.interactDistance;
    
    private void Start()
    {
        //This works locally but not in multiplayer, need a way to fix.
        object[] data = gameObject.GetPhotonView().instantiationData;
        farm = (Farm)data[0];
        Debug.Log(farm);
        farm.itemOnFarm = gameObject;
    }

    public bool IsInteractable => true;
    public GameObject GameObject => gameObject;

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
        FeedUI.Instance.AddFeedItem("Farm emptied", feedType: FeedItem.Type.World);
        farm.itemOnFarm = null;
        PhotonNetwork.Destroy(GameObject);

    }
}
