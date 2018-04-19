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

    private List<InventoryEntitySlot> inventorySlots;

    private void Start () {
        inventory = FindObjectOfType<Inventory>();
        equipmentManager = FindObjectOfType<EquipmentManager>();
        inventorySlots = new List<InventoryEntitySlot>(Inventory.InventorySize + Inventory.HotbarSize);

        InitializeHotbar();
        InitializeInventory();

        inventory.OnEntityChangedCallback += UpdateUI;
	}

    public void UpdateUI()
    {
        for (int i = Inventory.HotbarSize; i < Inventory.InventorySize + Inventory.HotbarSize; i++)
        {
            if (i < inventory.inventoryEntities.Count)
                inventorySlots[i].Entity = inventory.inventoryEntities[i];
            else inventorySlots[i].Clear();
        }

        for (int i = 0; i < Inventory.HotbarSize; i++)
        {
            if (i < inventory.inventoryEntities.Count)
                inventorySlots[i].Entity = inventory.inventoryEntities[i];
            else inventorySlots[i].Clear();
        }
    }

    private void InitializeHotbar()
    {
        for (int i = 0; i < Inventory.HotbarSize; i++)
        {
            var go = Instantiate(inventorySlotPrefab, hotbarUIGo.transform);
            inventorySlots.Add(go.GetComponentInChildren<InventoryEntitySlot>());
            inventorySlots[i].Initialize(i, inventory, equipmentManager);
        }
    }

    private void InitializeInventory()
    {
        for (int i = Inventory.HotbarSize; i < Inventory.HotbarSize + Inventory.InventorySize; i++)
        {
            var go = Instantiate(inventorySlotPrefab, inventoryUIGo.transform);
            inventorySlots.Add(go.GetComponentInChildren<InventoryEntitySlot>());
            inventorySlots[i].Initialize(i, inventory, equipmentManager);
        }
    }
}
