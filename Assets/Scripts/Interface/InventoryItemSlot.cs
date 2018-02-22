using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItemSlot : MonoBehaviour {
    private Inventory inventory;
    public Image icon;
    private Item item;

    public void AddItem(Item newItem)
    {
        item = newItem;
        icon.sprite = item.Sprite;
        icon.enabled = true;
    }

    public void Clear()
    {
        item = null;
        icon.sprite = null;
    }
}
