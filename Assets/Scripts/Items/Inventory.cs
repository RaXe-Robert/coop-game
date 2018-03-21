using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Utilities;
using System.Linq;

public class Inventory : MonoBehaviour
{
    public static readonly int InventorySize = 20;
    public static readonly int HotbarSize = 10;

    public ScriptableItemData diamond;
    public ScriptableItemData stick;
    public List<ItemBase> inventoryItems;
    private PhotonView photonView;
    private EquipmentManager equipmentManager;

    public delegate void OnItemChanged();
    public OnItemChanged OnItemChangedCallback;

    private void Start()
    {
        inventoryItems = new List<ItemBase>(new ItemBase[InventorySize + HotbarSize]);
        photonView = GetComponent<PhotonView>();
    }

    private void Update()
    {
        if (!photonView.isMine)
            return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            AddItemById(stick.Id, 10);
            AddItemById(diamond.Id, 10);
        }
    }

    private void AddNewItemStackById(int itemId, int stackSize)
    {
        ItemBase item = ItemFactory.CreateNewItem(itemId, stackSize);
        var emptyIndex = inventoryItems.FirstNullIndexAt();

        inventoryItems[emptyIndex.Value] = item;
        OnItemChangedCallback?.Invoke();
    }

    private void FillItemStacksById(int itemId, int stackSize)
    {
        ItemBase item = ItemFactory.CreateNewItem(itemId, stackSize);

        //Check if the item to add is a Resource item.
        if (item.GetType() == typeof(Resource))
        {
            int itemsToAdd = item.StackSize;

            //Get all the items in the inventory where the id is the same as the item to add id.
            var existingItems = inventoryItems.Where(x => x?.Id == item.Id && item.StackSize < ItemBase.MAXSTACKSIZE).ToArray();

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
                    int availableAmount = ItemBase.MAXSTACKSIZE - currentStack.StackSize;
                    if (availableAmount >= itemsToAdd)
                    {
                        currentStack.StackSize += itemsToAdd;
                        itemsToAdd = 0;
                        OnItemChangedCallback?.Invoke();
                    }
                    else
                    {
                        currentStack.StackSize = ItemBase.MAXSTACKSIZE;
                        itemsToAdd -= availableAmount;
                        OnItemChangedCallback?.Invoke();
                    }
                }
                if (itemsToAdd > 0)
                    AddNewItemStackById(itemId, itemsToAdd);
            }
            else
            {
                AddNewItemStackById(itemId, itemsToAdd);
            }
        }
        else
        {
            AddNewItemStackById(itemId, stackSize);
        }
    }

    /// <summary>
    /// Removes an Item from the inventory
    /// </summary>
    /// <param name="item"></param>
    public void RemoveItem(int index)
    {
        if (index < inventoryItems.Count && inventoryItems[index] != null)
        {
            inventoryItems[index] = null;
            OnItemChangedCallback?.Invoke();
        }
        else
            print($"Tried removing item at index {index} but it couldnt be found in the inventory");
    }

    public void SwapItem(int indexA, int indexB)
    {
        inventoryItems.Swap(indexA, indexB);
        OnItemChangedCallback?.Invoke();
    }

    public int GetItemAmountById(int itemId)
    {
        int temp = 0;
        for (int i = 0; i < inventoryItems.Count; i++)
        {
            if (inventoryItems[i]?.Id == itemId)
            {
                if (inventoryItems[i].GetType() == typeof(Resource))
                    temp += inventoryItems[i].StackSize;
                else temp += 1;
            }
        }
        return temp;
    }

    public bool CheckAmountById(int itemId, int amountNeeded)
    {
        return (GetItemAmountById(itemId) >= amountNeeded);
    }

    /// <summary>
    /// Removes items based on the item id and the amount of items to remove.
    /// THE ITEMS GET DESTROYED!!
    /// </summary>
    /// <param name="itemId">The id of the item to remove</param>
    /// <param name="amountToRemove">The amount of items to remove</param>
    public void RemoveItemById(int itemId, int amountToRemove = 1)
    {
        if (!CheckAmountById(itemId, amountToRemove))
        {
            Debug.LogError($"Inventory -- Trying to remove {amountToRemove} x item {itemId}, but we dont have that many");
            return;
        }

        //Remove items from inventory, start at the back of the inventory.
        //TODO: Only check the items with the required ID have to refactored removeItems and other things aswell
        for (int i = inventoryItems.Count - 1; i >= 0; i--)
        {
            //If all the items are removed we can stop
            if (amountToRemove == 0)
                return;

            if (inventoryItems[i]?.Id == itemId)
            {
                //Check if the item is a resource if so, we can take items of the stacksize.
                if (inventoryItems[i].GetType() == typeof(Resource))
                {
                    Resource currentStack = (Resource)inventoryItems[i];
                    if (amountToRemove >= currentStack.StackSize)
                    {
                        amountToRemove -= currentStack.StackSize;
                        RemoveItem(i);
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
                    RemoveItem(i);
                    OnItemChangedCallback?.Invoke();
                }
            }
        }
    }

    public void AddItemById(int itemId, int stackSize = 1)
    {
        if (!photonView.isMine)
            return;

        ItemBase item = ItemFactory.CreateNewItem(itemId, stackSize);
        if (!inventoryItems.FirstNullIndexAt().HasValue)
        {
            ItemFactory.CreateWorldObject(PlayerNetwork.PlayerObject.transform.position, item.Id, stackSize);
        }
        else
        {
            if (item.GetType() == typeof(Resource))
            {
                FillItemStacksById(itemId, stackSize);
            }
            else
            {
                AddNewItemStackById(itemId, stackSize);
            }
        }
    }

    /// <summary>
    /// Removes the required items for the craftingRecipe
    /// </summary>
    /// <param name="recipe">The recipe to craft</param>
    /// <returns>Whether there are enough materials to craft this recipe</returns>
    public bool RemoveItemsForCrafting(CraftingRecipe recipe)
    {
        for (int i = 0; i < recipe.requiredItems.Length; i++)
        {
            var requiredItem = recipe.requiredItems[i];
            if (!CheckAmountById(requiredItem.item.Id, requiredItem.amount * recipe.amountToCraft))
            {
                Debug.Log($"Not enough {requiredItem.item.name} to craft {recipe.result.item.name}");
                return false;
            }
        }

        for (int i = 0; i < recipe.requiredItems.Length; i++)
        {
            var requiredItem = recipe.requiredItems[i];
            RemoveItemById(requiredItem.item.Id, requiredItem.amount * recipe.amountToCraft);
        }

        return true;
    }

    public int GetMaxCrafts(CraftingRecipe recipe)
    {
        int maxCrafts = int.MaxValue;
        foreach (var craftingItem in recipe.requiredItems)
        {
            int temp = GetItemAmountById(craftingItem.item.Id) / craftingItem.amount;
            if (temp < maxCrafts)
                maxCrafts = temp;
        }
        return maxCrafts;
    }
}
