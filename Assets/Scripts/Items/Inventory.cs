using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour {

    public Item testItem;
    public List<Item> inventoryItems;
    public List<Item> hotBarItems;
    public int InventorySize { get; private set; }
    public int HotbarSize { get; private set; }

    public delegate void OnItemChanged();
    public OnItemChanged OnItemChangedCallback;

    private void Start()
    {
        InventorySize = 20;
        HotbarSize = 10;
        inventoryItems = new List<Item>();
        hotBarItems = new List<Item>();
    }

    private void Update()
    {
        if (!GetComponent<PhotonView>().isMine)
            return;

        if (Input.GetKeyDown(KeyCode.E))
            AddItem(testItem);
        if (Input.GetKeyDown(KeyCode.Q))
        {
            FindObjectOfType<ItemFactory>().CreateWorldObject(testItem, transform.position + Vector3.up);
            if(inventoryItems.Contains(testItem) || hotBarItems.Contains(testItem))
                RemoveItem(testItem);
        }
    }

    /// <summary>
    /// Adds an Item to the inventory
    /// </summary>
    /// <param name="item">The Item to add</param>
    /// <returns>Wether the item is added succesfully</returns>
    public bool AddItem(Item item)
    {
        if(hotBarItems.Count >= HotbarSize)
        {
            if (inventoryItems.Count >= InventorySize)
            {
                print($"Tried adding {item.name} but the inventory is full");
                return false; //Inventory is full
            }
            else
            {
                inventoryItems.Add(item);
                OnItemChangedCallback?.Invoke();
                return true;
            }
        }
        else
        {
            hotBarItems.Add(item);
            OnItemChangedCallback?.Invoke();
            return true;
        }
    }

    /// <summary>
    /// Removes an Item from the inventory
    /// </summary>
    /// <param name="item"></param>
    public void RemoveItem(Item item)
    {
        if (inventoryItems.Contains(item))
        {
            print($"Removed {item.name} from the inventory");
            inventoryItems.Remove(item);
            OnItemChangedCallback?.Invoke();
        }
        else
        {
            if (hotBarItems.Contains(item))
            {
                hotBarItems.Remove(item);
                OnItemChangedCallback?.Invoke();
            }
            else
            {
                print($"Tried removing {item.name} but it couldnt be found in the inventory");
            }
        }
    }
}
