using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour {

    public Item testItem;
    public List<Item> items;
    public int Size { get; private set; }

    public delegate void OnItemChanged();
    public OnItemChanged OnItemChangedCallback;

    private void Start()
    {
        Size = 20;
        items = new List<Item>();

        
    }

    private void Update()
    {
        if (!GetComponent<PhotonView>().isMine)
            return;
        if (Input.GetKeyDown(KeyCode.E))
            AddItem(testItem);
        if (Input.GetKeyDown(KeyCode.Q))
        {
            for (int i = 0; i < 1; i++)
            {
                FindObjectOfType<ItemFactory>().CreateWorldObject(testItem, new Vector3(0, 2, 0));
            }
        }
    }

    /// <summary>
    /// Adds an Item to the inventory
    /// </summary>
    /// <param name="item">The Item to add</param>
    /// <returns>Wether the item is added succesfully</returns>
    public bool AddItem(Item item)
    {
        if (items.Count >= Size)
        {
            print($"Tried adding {item.name} but the inventory is full");
            return false; //Inventory is full
        }
        else
        {
            items.Add(item);
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
        if (items.Contains(item))
        {
            print($"Removed {item.name} from the inventory");
            items.Remove(item);
            OnItemChangedCallback?.Invoke();
        }
        else
            print($"Tried removing {item.name} but it couldnt be found in the inventory");
    }
}
