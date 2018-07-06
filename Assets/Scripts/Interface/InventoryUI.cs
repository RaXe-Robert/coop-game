using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour {
    [SerializeField] private GameObject inventorySlotPrefab;

    [SerializeField] private GameObject inventoryUIGo;
    [SerializeField] private GameObject hotbarUIGo;

    private Inventory inventory;
    private EquipmentManager equipmentManager;

    private List<InventoryItemSlot> inventorySlots;
    private List<GameObject> hotbarSelectionSlots;

    private void Start () {
        inventory = FindObjectOfType<Inventory>();
        equipmentManager = FindObjectOfType<EquipmentManager>();
        inventorySlots = new List<InventoryItemSlot>(Inventory.InventorySize + Inventory.HotbarSize);
        hotbarSelectionSlots = new List<GameObject>();

        InitializeHotbar();
        InitializeInventory();

        inventory.OnItemChangedCallback += UpdateUI;
        inventory.OnHotbarChangedCallback += UpdateHotbarSelection;
	}

    public void UpdateUI()
    {
        for (int i = Inventory.HotbarSize; i < Inventory.InventorySize + Inventory.HotbarSize; i++)
        {
            if (i < inventory.inventoryItems.Count)
                inventorySlots[i].CurrentItem = inventory.inventoryItems[i];
            else inventorySlots[i].Clear();
        }

        for (int i = 0; i < Inventory.HotbarSize; i++)
        {
            if (i < inventory.inventoryItems.Count)
                inventorySlots[i].CurrentItem = inventory.inventoryItems[i];
            else inventorySlots[i].Clear();
        }
    }

    private void InitializeHotbar()
    {
        for (int i = 0; i < Inventory.HotbarSize; i++)
        {
            var go = Instantiate(inventorySlotPrefab, hotbarUIGo.transform);
            inventorySlots.Add(go.GetComponentInChildren<InventoryItemSlot>());
            inventorySlots[i].Initialize(i, inventory, equipmentManager);
            hotbarSelectionSlots.Add(inventorySlots[i].transform.parent.gameObject);
        }
    }

    private void InitializeInventory()
    {
        for (int i = Inventory.HotbarSize; i < Inventory.HotbarSize + Inventory.InventorySize; i++)
        {
            var go = Instantiate(inventorySlotPrefab, inventoryUIGo.transform);
            inventorySlots.Add(go.GetComponentInChildren<InventoryItemSlot>());
            inventorySlots[i].Initialize(i, inventory, equipmentManager);
        }
    }

    private void UpdateHotbarSelection(int num)
    {
        for (int i = 0; i < 10; i++)
        {
            if (num == i)
                hotbarSelectionSlots[i].GetComponent<Outline>().enabled = true;
            else
                hotbarSelectionSlots[i].GetComponent<Outline>().enabled = false;
        }
    }
}
