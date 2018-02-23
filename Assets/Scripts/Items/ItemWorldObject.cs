using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemWorldObject : MonoBehaviour {
    public Item item;

    /// <summary>
    /// Picks the item up and removes it from the world
    /// </summary>
    public void PickUp()
    {
        Debug.Log($"Picking up {item.name}");
        Destroy(gameObject);
    }
}
