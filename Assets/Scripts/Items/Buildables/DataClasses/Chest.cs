using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Chest : BuildableWorldObject
{
    public static readonly int ChestSize = 20;
    public List<Item> chestItems;

    private Animator animator;
    public bool IsOpened { get; private set; } = false;
    public bool CanControl = true;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        chestItems = new List<Item>(new Item[ChestSize]);
    }

    protected override UnityAction[] InitializeActions()
    {
        return new UnityAction[]
        {
            new UnityAction(OpenChest),
            new UnityAction(CloseChest)
        };
    }

    protected override void Pickup()
    {
        if (CanControl)
        {
            // If null the action will be cancelled
            if (BuildableInteractionMenu.Instance?.Target == null)
                return;

            BuildableInteractionMenu.Instance.Target.DestroyWorldObject();
        }
        else
        {
            FeedUI.Instance.AddFeedItem("Chest is occupied", feedType: FeedItem.Type.Error);
        }
    }

    private void OpenChest()
    {
        if (CanControl)
        {
            if (!IsOpened)
            {
                Debug.Log("Open");
                IsOpened = true;
                photonView.RPC("ChestOpenAnimation", PhotonTargets.All);
                
                //Open inventory ui and chest ui.                
                //Set chestitems on chest ui

                photonView.RPC("ToggleCanControl", PhotonTargets.Others);
            }
        }
        else
            FeedUI.Instance.AddFeedItem("Chest is occupied", feedType: FeedItem.Type.Error);
    }

    private void CloseChest()
    {
        if (CanControl)
        {
            if (IsOpened)
            {
                Debug.Log("Closed");
                IsOpened = false;
                photonView.RPC("ChestCloseAnimation", PhotonTargets.All);

                //Close chest ui

                photonView.RPC("ToggleCanControl", PhotonTargets.Others);
            }
        }
        else
        {
            FeedUI.Instance.AddFeedItem("Chest is occupied", feedType: FeedItem.Type.Error);
        }
    }

    [PunRPC]
    private void ToggleCanControl()
    {
        CanControl = !CanControl;
    } 
    
    [PunRPC]
    private void ChestOpenAnimation()
    {
        animator.SetBool("IsOpen", true);
    }

    [PunRPC]
    private void ChestCloseAnimation()
    {
        animator.SetBool("IsOpen", false);
    }
}
