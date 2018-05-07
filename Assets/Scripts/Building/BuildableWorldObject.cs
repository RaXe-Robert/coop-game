using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(PhotonView))]
public class BuildableWorldObject : Photon.MonoBehaviour, IInteractable
{
    public BuildableBase buildable;

    public float interactDistance = 1f;
    public List<UnityAction> Actions { get; private set; }

    private void Start()
    {
        Actions = new List<UnityAction>();
        
        if (buildable.Recoverable)
            Actions.Add(new UnityAction(Pickup));

        Actions.AddRange(InitializeActions());
    }

    protected void Pickup()
    {
        // If null the action will be cancelled
        if (BuildableInteractionMenu.Instance?.Target == null)
            return;

        BuildableInteractionMenu.Instance.Target.DestroyWorldObject();
    }

    protected virtual UnityAction[] InitializeActions()
    {
        return new UnityAction[0];
    }

    #region IInteractable Implementation

    public bool IsInteractable() => interactDistance > 0f;

    public void Interact(Vector3 invokerPosition)
    {
        if (!IsInteractable() || Vector3.Distance(transform.position, invokerPosition) > interactDistance)
            return;

        BuildableInteractionMenu bim = BuildableInteractionMenu.Instance;
        if (bim.TargetInstanceID != GetInstanceID())
            bim.Show(this, Actions?.ToArray());
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
        photonView.RPC("RPC_DestroyWorldObject", PhotonTargets.AllBuffered);
    }

    [PunRPC]
    protected void RPC_DestroyWorldObject()
    {
        Destroy(gameObject);
    }
}
