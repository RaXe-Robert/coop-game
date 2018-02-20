using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemWorldObject : MonoBehaviour {
    public Item item;

    void PickUp()
    {
        Debug.Log($"Picking up {item.name}");
        Destroy(gameObject);
    }
}
