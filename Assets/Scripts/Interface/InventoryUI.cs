using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour {

    [SerializeField] private GameObject inventoryUIGo;
    [SerializeField] private Inventory inventory;

    private InventoryItemSlot[] itemSlots;

	void Start () {
        //TODO: find a good way to get the right inventory when there are multiple players.
        inventory = FindObjectOfType<Inventory>();
        itemSlots = inventoryUIGo.GetComponentsInChildren<InventoryItemSlot>();
        inventory.OnItemChangedCallback += UpdateUI;
	}
	
	void Update () {
        if (Input.GetKeyDown(KeyCode.B) || Input.GetKeyDown(KeyCode.I))
            inventoryUIGo.SetActive(!inventoryUIGo.activeSelf);
	}

    private void UpdateUI()
    {
        for (int i = 0; i < itemSlots.Length; i++)
        {
            if (i < inventory.items.Count)
                itemSlots[i].AddItem(inventory.items[i]);
            else itemSlots[i].Clear();
        }
    }
}
