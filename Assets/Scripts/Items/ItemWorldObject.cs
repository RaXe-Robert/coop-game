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

    public bool IsInteractable()
    {
        return true;
    }

    public void Interact(Vector3 invokerPosition)
    {
        if (Vector3.Distance(transform.position, invokerPosition) > pickupDistance)
        {
            PlayerNetwork.PlayerObject.GetComponent<PlayerMovementController>().ItemToPickup = this.gameObject;
            return;
        }

        PlayerNetwork.PlayerObject.GetComponent<Inventory>().AddItemById(item.Id, item.StackSize);
        photonView.RPC(nameof(DestroyWorldObject), PhotonTargets.AllBuffered);
    }

    public string TooltipText()
    {
        return $"{item.Name} ({item.StackSize})";
    }

    #endregion //IInteractable Implementation
}
