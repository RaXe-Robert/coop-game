using Assets.Scripts.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Chest : BuildableWorldObject
{
    public static readonly int ChestSize = 40;   
    public List<Item> chestItems;
    public BuildingController BuildingController { get; private set; }
        
    private Animator animator;
    private string chestOccupiedMessage = "Chest is occupied";
    public bool IsOpened { get; private set; } = false;
    public bool CanControl = true;
    public GameObject owner;

    public delegate void OnItemChanged();
    public OnItemChanged OnItemChangedCallback;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        BuildingController = FindObjectOfType<BuildingController>();
    }

    private void Start()
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
        if (BuildableInteractionMenu.Instance.Target != this)
        {
            CloseChest();
            return;
        }             

        if (owner == null)
            return;

        if (!InRange(owner.transform.position))
        {
            CloseChest();
            owner = null;
            BuildableInteractionMenu.Instance.Hide();
        }        
    }

    public override void Interact(GameObject invoker)
    {
        if (!InRange(invoker.transform.position))
            return;
        
        var buildableInteractionMenu = BuildableInteractionMenu.Instance;
        if (buildableInteractionMenu.TargetInstanceID != GetInstanceID())
        {
            buildableInteractionMenu.Show(this, Actions?.ToArray());
            owner = invoker;
        }
        else
            buildableInteractionMenu.Hide();
    }

    protected override void Pickup()
    {
        if (CanControl)
        {            
            // If null the action will be cancelled
            if (BuildableInteractionMenu.Instance?.Target == null)
                return;

            CloseChest();
            BuildableInteractionMenu.Instance.Target.DestroyWorldObject();
            DropAllItems();
        }
        else
        {
            FeedUI.Instance.AddFeedItem(chestOccupiedMessage, feedType: FeedItem.Type.Error);
        }
    }

    private void OpenChest()
    {
        if (CanControl)
        {
            if (!IsOpened)
            {                
                IsOpened = true;
                photonView.RPC("ChestOpenAnimation", PhotonTargets.All);

                ChestUI.Instance.OpenChest(this);
                
                //Set chestitems on chest ui

                photonView.RPC("ToggleCanControl", PhotonTargets.Others);
            }
        }
        else
            FeedUI.Instance.AddFeedItem(chestOccupiedMessage, feedType: FeedItem.Type.Error);
    }

    private void CloseChest()
    {
        if (CanControl)
        {
            if (IsOpened)
            {
                IsOpened = false;
                photonView.RPC("ChestCloseAnimation", PhotonTargets.All);

                ChestUI.Instance.CloseChest();
                //Close chest ui
                if (chestItems != null)
                {
                    for (int i = 0; i < chestItems.Count; i++)
                    {
                        if (chestItems[i] != null)
                            photonView.RPC("SetChestItem", PhotonTargets.Others, i, chestItems[i].Id, chestItems[i].StackSize);
                        else
                            photonView.RPC("RemoveChestItem", PhotonTargets.Others, i);
                    }
                }
                photonView.RPC("ToggleCanControl", PhotonTargets.Others);
            }
        }
        else
        {
            FeedUI.Instance.AddFeedItem(chestOccupiedMessage, feedType: FeedItem.Type.Error);
        }
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
        if (item.GetType() == typeof(Resource))
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

                    Resource currentStack = existingItems[i] as Resource;
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
                if (chestItems[i].GetType() == typeof(Resource))
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
                if (chestItems[i].GetType() == typeof(Resource))
                {
                    Resource currentStack = (Resource)chestItems[i];
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
            if (item.GetType() == typeof(Resource))
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
            if (item.GetType() == typeof(Resource))
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
