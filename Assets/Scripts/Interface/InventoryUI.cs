using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour {

    [SerializeField] private GameObject inventoryUIGo;
    [SerializeField] private GameObject hotbarUIGo;
    [SerializeField] private Inventory inventory;

    private InventoryItemSlot[] inventorySlots;
    private InventoryItemSlot[] hotbarSlots;

    private void Start () {
        inventory = FindObjectOfType<Inventory>();
        inventorySlots = inventoryUIGo.GetComponentsInChildren<InventoryItemSlot>();
        hotbarSlots = hotbarUIGo.GetComponentsInChildren<InventoryItemSlot>();

        inventory.OnItemChangedCallback += UpdateUI;
	}
	
	private void Update () {
        if (Input.GetKeyDown(KeyCode.B) || Input.GetKeyDown(KeyCode.I))
            inventoryUIGo.SetActive(!inventoryUIGo.activeSelf);
	}

    private void UpdateUI()
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (i < inventory.inventoryItems.Count)
                inventorySlots[i].AddItem(inventory.inventoryItems[i]);
            else inventorySlots[i].Clear();
        }

        for (int i = 0; i < hotbarSlots.Length; i++)
        {
            if (i < inventory.hotBarItems.Count)
                hotbarSlots[i].AddItem(inventory.hotBarItems[i]);
            else hotbarSlots[i].Clear();
        }
    }
}
