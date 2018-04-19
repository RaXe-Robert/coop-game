using System;
using System.Collections;

using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(PhotonView))]
public class BuildableWorldObject : Photon.MonoBehaviour, IInteractable
{
    public BuildableBase buildable;

    public float interactDistance = 1f;

    #region IInteractable Implementation

    public bool IsInteractable() => interactDistance > 0f;

    public void Interact(Vector3 invokerPosition)
    {
        if (!IsInteractable() || Vector3.Distance(transform.position, invokerPosition) > interactDistance)
            return;

        BuildableInteractionMenu bim = BuildableInteractionMenu.Instance;
        if (bim.TargetInstanceID != GetInstanceID())
            bim.Show(this, buildable.Actions.ToArray());
        else
            bim.Hide();
    }

    public string TooltipText()
    {
        return $"{buildable.Name}";
    }

    #endregion //IInteractable Implementation
    
    public void DestroyWorldObject()
    {
        PlayerNetwork.PlayerObject.GetComponent<Inventory>().AddItemById(buildable.Id, buildable.StackSize);
        photonView.RPC(nameof(RPC_DestroyWorldObject), PhotonTargets.AllBuffered);
    }

    [PunRPC]
    private void RPC_DestroyWorldObject()
    {
        Destroy(gameObject);
    }
}
