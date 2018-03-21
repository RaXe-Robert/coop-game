using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour {
    [SerializeField] private GameObject inventorySlotPrefab;

    [SerializeField] private GameObject inventoryUIGo;
    [SerializeField] private GameObject craftingUIGo;
    [SerializeField] private GameObject equipmentUIGo;
    [SerializeField] private GameObject hotbarUIGo;
    [SerializeField] private Inventory inventory;

    private List<InventoryItemSlot> inventorySlots;

    private void Start () {
        inventory = FindObjectOfType<Inventory>();
        inventorySlots = new List<InventoryItemSlot>(Inventory.InventorySize + Inventory.HotbarSize);
        InitializeHotbar();
        InitializeInventory();
        inventory.OnItemChangedCallback += UpdateUI;
	}
	
	private void Update () {
        if (Input.GetKeyDown(KeyCode.B) || Input.GetKeyDown(KeyCode.I))
            inventoryUIGo.SetActive(!inventoryUIGo.activeSelf);
        if (Input.GetKeyDown(KeyCode.F))
            craftingUIGo.SetActive(!craftingUIGo.activeSelf);
        if (Input.GetKeyDown(KeyCode.C))
            equipmentUIGo.SetActive(!equipmentUIGo.activeSelf);

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
            inventorySlots[i].Initialize(i, inventory);
        }
    }

    private void InitializeInventory()
    {
        for (int i = Inventory.HotbarSize; i < Inventory.HotbarSize + Inventory.InventorySize; i++)
        {
            var go = Instantiate(inventorySlotPrefab, inventoryUIGo.transform);
            inventorySlots.Add(go.GetComponentInChildren<InventoryItemSlot>());
            inventorySlots[i].Initialize(i, inventory);
        }
    }
}
