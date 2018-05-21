﻿using Assets.Scripts.Utilities;

using UnityEngine;

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
    public bool InRange(Vector3 invokerPosition) => Vector3.Distance(invokerPosition, transform.position) < pickupDistance;

    public void Interact(GameObject invoker)
    {
        if (!InRange(invoker.transform.position))
            return;

        Inventory inventory = PlayerNetwork.PlayerObject.GetComponent<Inventory>();

        // Check if inventory is not full
        if (inventory.inventoryItems.FirstNullIndexAt().HasValue)
        {
            inventory.AddItemById(item.Id, item.StackSize);
            FeedUI.Instance.AddFeedItem("Picked up " + item.Name, item.Sprite, FeedItem.Type.Succes);
            photonView.RPC(nameof(DestroyWorldObject), PhotonTargets.AllBuffered);
        }
        else
            FeedUI.Instance.AddFeedItem("Inventory full", feedType: FeedItem.Type.Fail);
    }

    public string TooltipText()
    {
        return $"{item.Name} ({item.StackSize})";
    }

    #endregion //IInteractable Implementation
}
