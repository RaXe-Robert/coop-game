using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour {

    public Item testItem;
    private List<Item> items;
    private int size = 20;

    private void Start()
    {
        items = new List<Item>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
            AddItem(testItem);
        if (Input.GetKeyDown(KeyCode.Q))
            RemoveItem(testItem);
    }

    /// <summary>
    /// Adds an Item to the inventory
    /// </summary>
    /// <param name="item">The Item to add</param>
    /// <returns>Wether the item is added succesfully</returns>
    public bool AddItem(Item item)
    {
        if (items.Count >= size)
        {
            print($"Tried adding {item.name} but the inventory is full");
            return false; //Inventory is full
        }
        else
        {
            items.Add(item);
            return true;
        }
    }

    /// <summary>
    /// Removes an Item from the inventory
    /// </summary>
    /// <param name="item"></param>
    public void RemoveItem(Item item)
    {
        if (items.Contains(item))
        {
            print($"Removed {item.name} from the inventory");
            items.Remove(item);
        }
        else
            print($"Tried removing {item.name} but it couldnt be found in the inventory");
    }
}
