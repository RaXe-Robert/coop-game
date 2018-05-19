using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(PhotonView))]
public class ItemWorldObject : Photon.MonoBehaviour, IInteractable
{
    public Item item;
    public float pickupDistance = 1f;

    [PunRPC]
    public void DestroyWorldObject()
    {
        Destroy(gameObject);
    }

    #region IInteractable Implementation

    public bool IsInteractable => true;
    public GameObject GameObject => gameObject;
    public bool InRange(Vector3 invokerPosition) =>
        Vector3.Distance(invokerPosition, transform.position) < pickupDistance;

    public void Interact(GameObject invoker)
    {
        if (!InRange(invoker.transform.position))
            return;

        //FeedUI.Instance.AddFeedItem("Trying to pick up " + item.Name, item.Sprite, FeedItem.Type.Fail);
        FeedUI.Instance.AddFeedItem("Picked up " + item.Name, item.Sprite, FeedItem.Type.Succes);
        //FeedUI.Instance.AddFeedItem("You need another tool to chop " + item.Name, feedType: FeedItem.Type.World);
        //FeedUI.Instance.AddFeedItem("Something went wrong", feedType: FeedItem.Type.Error);
        //FeedUI.Instance.AddFeedItem("No FeedItem.Type selected" + item.Name, item.Sprite);
        //FeedUI.Instance.AddFeedItem("This is the default color", item.Sprite, FeedItem.Type.Default);

        PlayerNetwork.PlayerObject.GetComponent<Inventory>().AddItemById(item.Id, item.StackSize);
        photonView.RPC(nameof(DestroyWorldObject), PhotonTargets.AllBuffered);
    }

    public string TooltipText()
    {
        return $"{item.Name} ({item.StackSize})";
    }

    #endregion //IInteractable Implementation
}
