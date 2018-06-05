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
    public List<UnityAction> Actions { get; set; }

    private void Start()
    {
        Actions = new List<UnityAction>();
        
        if (buildable.Recoverable)
            Actions.Add(new UnityAction(Pickup));

        Actions.AddRange(InitializeActions());
    }

    protected virtual void Pickup()
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

    public bool IsInteractable => true;
    public GameObject GameObject => gameObject;

    public bool InRange(Vector3 invokerPosition) =>
        Vector3.Distance(invokerPosition, transform.position) < interactDistance;

    public void Interact(GameObject invoker)
    {
        if (!InRange(invoker.transform.position))
            return;

        var buildableInteractionMenu = BuildableInteractionMenu.Instance;
        if (buildableInteractionMenu.TargetInstanceID != GetInstanceID())
            buildableInteractionMenu.Show(this, Actions?.ToArray());
        else
            buildableInteractionMenu.Hide();
    }

    public string TooltipText => $"{buildable.Name}";

    #endregion //IInteractable Implementation

    public void DestroyWorldObject()
    {
        PlayerNetwork.LocalPlayer.GetComponent<Inventory>().AddItemById(buildable.Id, buildable.StackSize);
        photonView.RPC("RPC_DestroyWorldObject", PhotonTargets.AllBuffered);
    }

    [PunRPC]
    protected void RPC_DestroyWorldObject()
    {
        Destroy(gameObject);
    }
}
