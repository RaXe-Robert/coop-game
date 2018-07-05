using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(PhotonView))]
public class BuildableWorldObject : Photon.MonoBehaviour, IInteractable
{
    public BuildableBase buildable;

    private Inventory inventory;
    private bool inUse = false;

    public float interactDistance = 1f;
    public List<UnityAction> Actions { get; set; }

    protected virtual void Start()
    {

        Actions = new List<UnityAction>();
        
        if (buildable.Recoverable)
            Actions.Add(new UnityAction(Pickup));

        Actions.AddRange(InitializeActions());
    }

    protected virtual void Pickup()
    {
        DestroyWorldObject();
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

    public virtual void Interact(GameObject invoker, Item item)
    {
        if (!InRange(invoker.transform.position))
            return;
        
        if (item?.GetType() == typeof(Tool))
        {
            Tool tool = item as Tool;
            if (tool.ToolType == ToolType.Hammer)
                Actions[0].Invoke();
        }
        else if (Actions.Count > 1)
        {
            if (inUse == false)
            {
                Actions[1].Invoke();
                inUse = true;
            }
            else if (Actions.Count > 2)
            {
                Actions[2].Invoke();
                inUse = false;
            }
        }
    }

    public string TooltipText => $"{buildable.Name}";

    #endregion //IInteractable Implementation

    public void DestroyWorldObject()
    {
        ItemFactory.CreateWorldObject(transform.position, buildable.Id, buildable.StackSize);
        FeedUI.Instance.AddFeedItem($"{buildable.Name} demolished", buildable.Sprite, FeedItem.Type.World);
        photonView.RPC("RPC_DestroyWorldObject", PhotonTargets.AllBuffered);
    }

    [PunRPC]
    protected void RPC_DestroyWorldObject()
    {
        Destroy(gameObject);
    }
}
