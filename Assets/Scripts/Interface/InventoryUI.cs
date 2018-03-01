using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour {
    public static readonly int inventorySize = 20;
    public static readonly int hotbarSize = 10;

    [SerializeField] private GameObject inventorySlotPrefab;

    [SerializeField] private GameObject inventoryUIGo;
    [SerializeField] private GameObject hotbarUIGo;
    [SerializeField] private Inventory inventory;

    private InventoryItemSlot[] inventorySlots;
    private InventoryItemSlot[] hotbarSlots;

    private void Start () {
        inventory = FindObjectOfType<Inventory>();
        inventorySlots = inventoryUIGo.GetComponentsInChildren<InventoryItemSlot>();
        hotbarSlots = hotbarUIGo.GetComponentsInChildren<InventoryItemSlot>();

        //Setup hotbar
        for (int i = 0; i < 10; i++)
        {
            var go = Instantiate(inventorySlotPrefab, hotbarUIGo.transform);
            go.GetComponent<InventoryItemSlot>().Initialize(i, inventory);
        }

        //Setup inventory
        for (int i = 0; i < 20; i++)
        {
            var go = Instantiate(inventorySlotPrefab, inventoryUIGo.transform);
            go.GetComponent<InventoryItemSlot>().Initialize(i + 10, inventory);
        }

        inventory.OnItemChangedCallback += UpdateUI;
	}
	
	private void Update () {
        if (Input.GetKeyDown(KeyCode.B) || Input.GetKeyDown(KeyCode.I))
            inventoryUIGo.SetActive(!inventoryUIGo.activeSelf);
	}

    public void UpdateUI()
    {
        for (int i = hotbarSlots.Length; i < inventorySize; i++)
        {
            if (i < inventory.inventoryItems.Count)
                inventorySlots[i].Item = inventory.inventoryItems[i];
            else inventorySlots[i].Clear();
        }

        for (int i = 0; i < hotbarSlots.Length; i++)
        {
            if (i < inventory.inventoryItems.Count)
                hotbarSlots[i].Item = inventory.inventoryItems[i];
            else hotbarSlots[i].Clear();
        }
    }
}
