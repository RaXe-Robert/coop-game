﻿using Assets.Scripts.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Chest : BuildableWorldObject
{
    public const int ChestSize = 40;   
    public List<Item> chestItems;
    public BuildingController BuildingController { get; private set; }
        
    private Animator animator;
    public bool IsOpened => animator.GetBool("IsOpen");

    private PhotonPlayer user;
    private bool IsLocalPlayerUser => user == PhotonNetwork.player;

    public delegate void OnItemChanged();
    public OnItemChanged OnItemChangedCallback;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        BuildingController = FindObjectOfType<BuildingController>();
    }

    protected override void Start()
    {
        chestItems = new List<Item>(new Item[ChestSize]);
        Actions = new List<UnityAction>();

        if (buildable.Recoverable)
            Actions.Add(new UnityAction(Pickup));

        Actions.AddRange(InitializeActions());
    }

    protected override UnityAction[] InitializeActions()
    {
        return new UnityAction[]
        {
            new UnityAction(OpenChest),
            new UnityAction(CloseChest)
        };
    }

    public void Update()
    {
        if (user == null)
            return;

        if (IsOpened && IsLocalPlayerUser)
        {
            if(!InRange(PlayerNetwork.LocalPlayer.gameObject.transform.position))
            {
                CloseChest();
                return;
            }
        }
    }

    protected override void Pickup()
    {
        if (user == null || IsLocalPlayerUser)
        {            
            CloseChest();
            DestroyWorldObject();
            DropAllItems();
        }
        else
            FeedUI.Instance.AddFeedItem($"Chest is occupied by {user.NickName}", feedType: FeedItem.Type.Error);
    }

    private void OpenChest()
    {
        if (IsOpened)
        {
            if (!IsLocalPlayerUser)
                FeedUI.Instance.AddFeedItem($"Chest is occupied by {user.NickName}", feedType: FeedItem.Type.Error);
            return;
        }

        if (user == null)
        {       
            photonView.RPC(nameof(ChestOpenAnimation), PhotonTargets.AllBuffered);
            ChestUI.Instance.OpenChest(this);
            photonView.RPC(nameof(SetUser), PhotonTargets.AllBuffered, PhotonNetwork.player.ID);
        }
        else
            FeedUI.Instance.AddFeedItem($"Chest is occupied by {user.NickName}", feedType: FeedItem.Type.Error);
    }

    private void CloseChest()
    {
        if (!IsOpened)
            return;
            
        if (IsLocalPlayerUser)
        {
            photonView.RPC(nameof(ChestCloseAnimation), PhotonTargets.AllBuffered);
            ChestUI.Instance.CloseChest();
            if (chestItems != null)
            {
                for (int i = 0; i < chestItems.Count; i++)
                {
                    if (chestItems[i] != null)
                        photonView.RPC(nameof(SetChestItem), PhotonTargets.OthersBuffered, i, chestItems[i].Id, chestItems[i].StackSize);
                    else
                        photonView.RPC(nameof(RemoveChestItem), PhotonTargets.OthersBuffered, i);
                }
            }
            photonView.RPC(nameof(SetUser), PhotonTargets.AllBuffered, -1);
        }
        else
            FeedUI.Instance.AddFeedItem($"Chest is occupied by {user.NickName}", feedType: FeedItem.Type.Error);
    }

    private void AddNewItemStackById(string itemId, int stackSize)
    {
        //If inventory is full we drop the items on the floor
        if (!chestItems.FirstNullIndexAt().HasValue)
        {
            ItemFactory.CreateWorldObject(transform.position, itemId, stackSize);
            return;
        }

        Item item = ItemFactory.CreateNewItem(itemId, stackSize);
        var emptyIndex = chestItems.FirstNullIndexAt();

        chestItems[emptyIndex.Value] = item;
        OnItemChangedCallback?.Invoke();
    }

    private void FillItemStacksById(string itemId, int stackSize)
    {
        Item item = ItemFactory.CreateNewItem(itemId, stackSize);

        //Check if the item to add is a Resource item.
        if (item.GetType() == typeof(Resource) || item.GetType() == typeof(Consumable))
        {
            int itemsToAdd = item.StackSize;

            //Get all the items in the inventory where the id is the same as the item to add id.
            var existingItems = chestItems.Where(x => x?.Id == item.Id && item.StackSize < Item.MAXSTACKSIZE).ToArray();

            //There are uncompleted stacks, we can add items to them.
            if (existingItems != null)
            {
                //Loop through all the existing item stacks and check if there is any room.
                for (int i = 0; i < existingItems.Length; i++)
                {
                    //We should be done adding items, return
                    if (itemsToAdd == 0)
                        return;

                    Item currentStack = existingItems[i];
                    int availableAmount = Item.MAXSTACKSIZE - currentStack.StackSize;
                    if (availableAmount >= itemsToAdd)
                    {
                        currentStack.StackSize += itemsToAdd;
                        itemsToAdd = 0;
                        OnItemChangedCallback?.Invoke();
                    }
                    else
                    {
                        currentStack.StackSize = Item.MAXSTACKSIZE;
                        itemsToAdd -= availableAmount;
                        OnItemChangedCallback?.Invoke();
                    }
                }
                if (itemsToAdd > 0)
                    AddNewItemStackById(itemId, itemsToAdd);
            }
            else
                AddNewItemStackById(itemId, itemsToAdd);
        }
        else
            AddNewItemStackById(itemId, stackSize);
    }

    /// <summary>
    /// Removes an item from the inventory
    /// </summary>
    public void RemoveItemAtIndex(int index)
    {
        if (index < chestItems.Count && chestItems[index] != null)
        {
            chestItems[index] = null;
            OnItemChangedCallback?.Invoke();
        }
        else
            print($"Tried removing an item at index {index} but it couldnt be found in the inventory");
    }

    public void SwapItems(int indexA, int indexB)
    {
        chestItems.Swap(indexA, indexB);
        OnItemChangedCallback?.Invoke();
    }

    public int GetItemAmountById(string itemId)
    {
        if (IsChestEmpty())
            return 0;

        int temp = 0;
        for (int i = 0; i < chestItems.Count; i++)
        {
            if (chestItems[i]?.Id == itemId)
            {
                if (chestItems[i].GetType() == typeof(Resource) || chestItems[i].GetType() == typeof(Consumable))
                    temp += chestItems[i].StackSize;
                else temp += 1;
            }
        }
        return temp;
    }
    private bool IsChestEmpty()
    {
        return chestItems == null;
    }

    public bool CheckAmountById(string itemId, int amountNeeded)
    {
        return (GetItemAmountById(itemId) >= amountNeeded);
    }

    /// <summary>
    /// Removes items based on the item id and the amount of items to remove. starting at the start of the inventory
    /// THE items GET DESTROYED!!
    /// </summary>
    /// <param name="itemId">The id of the item to remove</param>
    /// <param name="amountToRemove">The amount of items to remove</param>
    public void RemoveItemById(string itemId, int amountToRemove = 1)
    {
        if (!CheckAmountById(itemId, amountToRemove))
        {
            Debug.LogError($"Inventory -- Trying to remove {amountToRemove} x item {itemId}, but we dont have that many");
            return;
        }

        //Remove items from inventory, start at the back of the inventory.
        //TODO: Only check the items with the required ID have to refactored items and other things aswell
        for (int i = 0; i < chestItems.Count; i++)
        {
            //If all the items are removed we can stop
            if (amountToRemove == 0)
                return;

            if (chestItems[i]?.Id == itemId)
            {
                //Check if the item is a resource if so, we can take items of the stacksize.
                if (chestItems[i].GetType() == typeof(Resource) || chestItems[i].GetType() == typeof(Consumable))
                {
                    Item currentStack = chestItems[i];
                    if (amountToRemove >= currentStack.StackSize)
                    {
                        amountToRemove -= currentStack.StackSize;
                        RemoveItemAtIndex(i);
                    }
                    else
                    {
                        currentStack.StackSize -= amountToRemove;
                        amountToRemove = 0;
                        OnItemChangedCallback?.Invoke();
                        return;
                    }
                }
                //If it aint a resource we just get the single item.
                else
                {
                    amountToRemove--;
                    RemoveItemAtIndex(i);
                    OnItemChangedCallback?.Invoke();
                }
            }
        }
    }

    public void AddItemById(string itemId, int stackSize = 1)
    {
        if (!photonView.isMine)
            return;

        Item item = ItemFactory.CreateNewItem(itemId, stackSize);

        if (!chestItems.FirstNullIndexAt().HasValue)
        {
            //Check if we are adding a resource item, if so we check if we have full stacks of the item.
            if (item.GetType() == typeof(Resource) || item.GetType() == typeof(Consumable))
            {
                if (GetItemAmountById(item.Id) % 64 != 0)
                    FillItemStacksById(itemId, stackSize);
                else
                    ItemFactory.CreateWorldObject(PlayerNetwork.LocalPlayer.transform.position, item.Id, stackSize);
            }
            else
                ItemFactory.CreateWorldObject(PlayerNetwork.LocalPlayer.transform.position, item.Id, stackSize);
        }
        else
        {
            if (item.GetType() == typeof(Resource) || item.GetType() == typeof(Consumable))
                FillItemStacksById(itemId, stackSize);
            else
                AddNewItemStackById(itemId, stackSize);
        }

        SoundManager.Instance.PlaySound(SoundManager.Sound.PICKUP);
    }

    public void AddItemAtIndex(string itemId, int index, int stackSize = 1)
    {
        if (index < 0 || chestItems[index] != null)
        {
            Debug.LogError("Chest -- AddItemAtIndex -- invalid index");
            AddItemById(itemId, stackSize);
        }
        else
        {
            Item item = ItemFactory.CreateNewItem(itemId, stackSize);
            chestItems[index] = item;
            OnItemChangedCallback?.Invoke();
        }
    } 

    public void DropAllItems()
    {
        for (int i = 0; i < chestItems.Count; i++)
        {
            var item = chestItems[i];
            if (item == null)
                continue;

            var position = transform.position + new Vector3(Random.Range(-2, 2), 0, Random.Range(-2, 2));
            ItemFactory.CreateWorldObject(position, item.Id, item.StackSize);
            chestItems[i] = null;
        }
        OnItemChangedCallback?.Invoke();
    }

    private void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
    {
        if (IsOpened && PhotonNetwork.isMasterClient)
        {
            if (user == otherPlayer)
            {
                user = PhotonNetwork.player;
                CloseChest();
            }
        }
    }

    [PunRPC]
    private void SetUser(int userID)
    {
        if (userID == -1)
        {
            user = null;
        }
        else
        {
            PhotonPlayer[] players = PhotonNetwork.playerList;
            for (int i = 0; i < players.Length; i++)
            {
                if (userID == players[i].ID)
                {
                    user = players[i];
                }
            }
        }
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

    [PunRPC]
    private void SetChestItem(int index, string id, int stacksize)
    {
        chestItems[index] = ItemFactory.CreateNewItem(id,stacksize);
    }

    [PunRPC]
    private void RemoveChestItem(int index)
    {
        chestItems[index] = null;
    }
}
