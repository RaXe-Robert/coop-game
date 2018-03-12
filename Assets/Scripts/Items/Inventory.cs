using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Utilities;

public class Inventory : MonoBehaviour
{
    public static readonly int InventorySize = 20;
    public static readonly int HotbarSize = 10;
    public ItemData testItem;
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
            AddItem(ItemFactory.CreateNewItem(testItem.Id));

        if (Input.GetKeyDown(KeyCode.Q))
            FindObjectOfType<ItemFactory>().CreateWorldObject(ItemFactory.CreateNewItem(testItem.Id), transform.position + Vector3.up);
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
            print("Inventory is full");
            return false;
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
            if(inventoryItems[i].Id == itemId)
            {
                if (inventoryItems[i].GetType() == typeof(Resource))
                    temp += ((Resource)inventoryItems[i]).StackSize;
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
        for (int i = inventoryItems.Count; i > 0; --i)
        {
            if (amountToRemove == 0)
                return;

            if(inventoryItems[i].Id == itemId)
            {
                if (inventoryItems[i].GetType() == typeof(ResourceData))
                {
                    Resource temp = (Resource)inventoryItems[i];
                    if (amountToRemove >= temp.StackSize)
                    {
                        amountToRemove -= temp.StackSize;
                        RemoveItem(i);
                    }
                    else
                    {
                        temp.StackSize -= amountToRemove;
                        OnItemChangedCallback?.Invoke();
                        return;
                    }
                }
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

        }
    }
}
