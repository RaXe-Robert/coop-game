using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public Item testItem;
    public List<Item> inventoryItems;

    public int InventorySize { get; private set; }
    public int HotbarSize { get; private set; }

    public delegate void OnItemChanged();
    public OnItemChanged OnItemChangedCallback;

    private void Start()
    {
        InventorySize = 20;
        HotbarSize = 10;
        inventoryItems = new List<Item>();
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
        if (inventoryItems.Count >= InventorySize)
        {
            print("Inventory is full");
            return false;
        }
        else
        {
            inventoryItems.Add(item);
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
            inventoryItems.RemoveAt(index);
            OnItemChangedCallback?.Invoke();
        }
        else
            print($"Tried removing item at index {index} but it couldnt be found in the inventory");
    }
}
