using UnityEngine;
using System;
using UnityEngine.EventSystems;

public class ArmorItemSlot : InventoryItemSlot
{
    [SerializeField] private ArmorType slotType;
    public ArmorType SlotType { get { return slotType; } }

    public override void OnDrop(PointerEventData eventData)
    {
        InventoryItemSlot from;
        if ((from = eventData.pointerDrag.GetComponent<InventoryItemSlot>()))
        {
            if (from.CurrentItem.GetType() == typeof(Armor))
                equipmentManager.EquipArmor(from.CurrentItem as Armor, from.index);
        }
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Right)
            return;

        InventoryItemSlot from;
        if ((from = eventData.pointerDrag.GetComponent<InventoryItemSlot>()))
        {
            if (from.CurrentItem.GetType() == typeof(Armor))
                equipmentManager.UnequipItem(from.CurrentItem as Armor, from.index);
            else if (from.CurrentItem.GetType() == typeof(Tool))
                equipmentManager.UnequipItem(from.CurrentItem as Tool, from.index);
            else if (from.CurrentItem.GetType() == typeof(Weapon))
                equipmentManager.UnequipItem(from.CurrentItem as Weapon, from.index);
        }

        //if(inventory.inventoryItems.Capacity > 0) // Inventory.InventorySize)
        //{
        //    Debug.Log(inventory.inventoryItems.Capacity);
        //    //Get inventory space
        //    // add to inventory
        //}
        //else
        //    equipmentManager.DropEquippedItem(CurrentItem as Item);
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
        transform.SetParent(initialParentTransform);
        transform.localPosition = Vector3.zero;

        if (!EventSystem.current.IsPointerOverGameObject())
            equipmentManager.DropEquippedItem(CurrentItem as Item);
    }
}

