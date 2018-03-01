using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Utilities;

public class Inventory : MonoBehaviour
{
    public static readonly int InventorySize = 20;
    public static readonly int HotbarSize = 10;
    public Item testItem;
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
            AddItem(testItem);

        if (Input.GetKeyDown(KeyCode.Q))
            FindObjectOfType<ItemFactory>().CreateWorldObject(testItem, transform.position + Vector3.up);
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
}
