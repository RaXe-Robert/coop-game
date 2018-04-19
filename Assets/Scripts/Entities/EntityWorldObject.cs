using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(PhotonView))]
public class EntityWorldObject : Photon.MonoBehaviour, IInteractable
{
    public EntityBase entity;
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

        PlayerNetwork.PlayerObject.GetComponent<Inventory>().AddEntityById(entity.Id, entity.StackSize);
        photonView.RPC(nameof(DestroyWorldObject), PhotonTargets.AllBuffered);
    }

    public string TooltipText()
    {
        return $"{entity.Name} ({entity.StackSize})";
    }

    #endregion //IInteractable Implementation
}
