using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestUI : MonoBehaviour {
    [SerializeField] private GameObject inventorySlotPrefab;

    private Chest chest;
    private List<InventoryItemSlot> inventorySlots;

    private void InitializeChest()
    {
        for (int i = 0; i < Chest.ChestSize; i++)
        {
            var go = Instantiate(inventorySlotPrefab, transform);
            inventorySlots.Add(go.GetComponentInChildren<InventoryItemSlot>());
            //inventorySlots[i].Initialize(i, inventory, equipmentManager);
        }
    }

    public void SetChest(Chest c)
    {
        chest = c;
        InitializeChest();
        gameObject.SetActive(true);
    }
}
