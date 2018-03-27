﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour {
    [SerializeField] private GameObject inventorySlotPrefab;

    [SerializeField] private GameObject inventoryUIGo;
    [SerializeField] private GameObject hotbarUIGo;

    private Inventory inventory;
    private EquipmentManager equipmentManager;
    private BuildingController buildingController;

    private List<InventoryItemSlot> inventorySlots;

    private void Start () {
        inventory = FindObjectOfType<Inventory>();
        equipmentManager = FindObjectOfType<EquipmentManager>();
        buildingController = FindObjectOfType<BuildingController>();
        inventorySlots = new List<InventoryItemSlot>(Inventory.InventorySize + Inventory.HotbarSize);

        InitializeHotbar();
        InitializeInventory();

        inventory.OnItemChangedCallback += UpdateUI;
	}

    public void UpdateUI()
    {
        for (int i = Inventory.HotbarSize; i < Inventory.InventorySize + Inventory.HotbarSize; i++)
        {
            if (i < inventory.inventoryItems.Count)
                inventorySlots[i].Item = inventory.inventoryItems[i];
            else inventorySlots[i].Clear();
        }

        for (int i = 0; i < Inventory.HotbarSize; i++)
        {
            if (i < inventory.inventoryItems.Count)
                inventorySlots[i].Item = inventory.inventoryItems[i];
            else inventorySlots[i].Clear();
        }
    }

    private void InitializeHotbar()
    {
        for (int i = 0; i < Inventory.HotbarSize; i++)
        {
            var go = Instantiate(inventorySlotPrefab, hotbarUIGo.transform);
            inventorySlots.Add(go.GetComponentInChildren<InventoryItemSlot>());
            inventorySlots[i].Initialize(i, inventory, equipmentManager, buildingController);
        }
    }

    private void InitializeInventory()
    {
        for (int i = Inventory.HotbarSize; i < Inventory.HotbarSize + Inventory.InventorySize; i++)
        {
            var go = Instantiate(inventorySlotPrefab, inventoryUIGo.transform);
            inventorySlots.Add(go.GetComponentInChildren<InventoryItemSlot>());
            inventorySlots[i].Initialize(i, inventory, equipmentManager, buildingController);
        }
    }
}
