using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Utilities;
using System.Linq;

public class Inventory : MonoBehaviour
{
    public static readonly int InventorySize = 20;
    public static readonly int HotbarSize = 10;
    public ItemData diamond;
    public ItemData stick;
    public List<Item> inventoryItems;

    public delegate void OnItemChanged();
    public OnItemChanged OnItemChangedCallback;

    private void Start()
    {
        inventoryItems = new List<Item>(new Item[InventorySize + HotbarSize]);
    }

    private void Update()
    {
        if (!GetComponent<PhotonView>().isMine)
            return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            AddItem(ItemFactory.CreateNewItem(stick.Id));
            AddItem(ItemFactory.CreateNewItem(diamond.Id));
        }

        if (Input.GetKeyDown(KeyCode.Q))
            FindObjectOfType<ItemFactory>().CreateWorldObject(ItemFactory.CreateNewItem(diamond.Id), transform.position + Vector3.up);
    }

    /// <summary>
    /// Adds an Item to the inventory
    /// </summary>
    /// <param name="item">The Item to add</param>
    /// <returns>Wether the item is added succesfully</returns>
    public bool AddItem(Item item)
    {
        var emptyIndex = inventoryItems.FirstNullIndexAt();
        if (!emptyIndex.HasValue)
        {
            //TODO: drop newly added item to the floor
            print("Inventory is full");
            return false;
        }
        if (item.GetType() == typeof(Resource))
        {
            int itemsToAdd = ((Resource)item).Amount;

            //If we already have an item with this id in inventory we can check if we can add it to that item.
            if (CheckAmountById(item.Id, 1))
            {
                var notCompleteStacks = inventoryItems.Where(x => x?.Id == item.Id).ToArray();

                //There are uncompleted stacks, we can add some items to them.
                if (notCompleteStacks != null)
                {
                    for (int i = 0; i < notCompleteStacks.Length; i++)
                    {
                        if (itemsToAdd == 0)
                            return true;

                        Resource currentStack = inventoryItems[i] as Resource;
                        int availableAmount = Resource.STACKSIZE - currentStack.Amount;
                        if (availableAmount >= itemsToAdd)
                        {
                            currentStack.Amount += itemsToAdd;
                            itemsToAdd = 0;
                            OnItemChangedCallback?.Invoke();
                        }
                        else
                        {
                            currentStack.Amount = Resource.STACKSIZE;
                            itemsToAdd -= availableAmount;
                        }
                    }
                    return true;
                }
                //There are currenctly no uncompleted stacks, create a new stack.
                else
                {
                    inventoryItems[emptyIndex.Value] = item;
                    OnItemChangedCallback?.Invoke();
                    return true;
                }
            }
            else
            {
                inventoryItems[emptyIndex.Value] = item;
                OnItemChangedCallback?.Invoke();
                return true;
            }
        }
        else
        {
            inventoryItems[emptyIndex.Value] = item;
            OnItemChangedCallback?.Invoke();
            return true;
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
                    temp += ((Resource)inventoryItems[i]).Amount;
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
    public void RemoveItemById(int itemId, int amountToRemove)
    {
        if (!CheckAmountById(itemId, amountToRemove))
        {
            Debug.Log($"Inventory -- Trying to remove {amountToRemove} x item {itemId}, but we dont have that many");
            return;
        }

        //Remove items from inventory, start at the back of the inventory.
        for (int i = inventoryItems.Count - 1; i > 0; i--)
        {
            //If all the items are removed we can stop
            if (amountToRemove == 0)
                return;

            if (inventoryItems[i]?.Id == itemId)
            {
                //Check if the item is a resource if so, we can take items of the stacksize.
                if (inventoryItems[i].GetType() == typeof(ResourceData))
                {
                    Resource temp = (Resource)inventoryItems[i];
                    if (amountToRemove >= temp.Amount)
                    {
                        amountToRemove -= temp.Amount;
                        RemoveItem(i);
                    }
                    else
                    {
                        temp.Amount -= amountToRemove;
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

    public void AddItemById(int itemId, int amountToAdd)
    {
        if (!inventoryItems.FirstNullIndexAt().HasValue)
        {
            //Inventory is full, dropping the items that should be added to the inventory.
            //TODO: Drop items
        }
        else
        {
            AddItem(ItemFactory.CreateNewItem(itemId, amountToAdd));
            OnItemChangedCallback?.Invoke();
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
            if (!CheckAmountById(requiredItem.item.Id, requiredItem.amount))
            {
                Debug.Log($"Not enough {requiredItem.item.name} to craft {recipe.resultItem.item.name}");
                return false;
            }
        }

        for (int i = 0; i < recipe.requiredItems.Length; i++)
        {
            var requiredItem = recipe.requiredItems[i];
            RemoveItemById(requiredItem.item.Id, requiredItem.amount);
        }

        return true;
    }
}
