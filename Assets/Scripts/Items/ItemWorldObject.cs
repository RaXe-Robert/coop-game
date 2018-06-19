using Assets.Scripts.Utilities;

using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class ItemWorldObject : Photon.MonoBehaviour, IInteractable
{
    public Item Item;
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

    public void Interact(GameObject invoker, Item interactionItem)
    {
        if (!InRange(invoker.transform.position))
            return;

        Inventory inventory = PlayerNetwork.LocalPlayer.GetComponent<Inventory>();

        // Check if inventory is not full
        if (inventory.inventoryItems.FirstNullIndexAt().HasValue)
        {
            inventory.AddItemById(Item.Id, Item.StackSize);
            FeedUI.Instance.AddFeedItem("Picked up " + Item.Name, Item.Sprite, FeedItem.Type.Succes);            
            photonView.RPC(nameof(DestroyWorldObject), PhotonTargets.AllBuffered);
        }
        else
            FeedUI.Instance.AddFeedItem("Inventory full", feedType: FeedItem.Type.Fail);
    }

    public string TooltipText => $"{Item.Name} ({Item.StackSize})";

    #endregion //IInteractable Implementation
}
