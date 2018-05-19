using UnityEngine;
using System;
using UnityEngine.EventSystems;
using Assets.Scripts.Utilities;

public class WeaponItemSlot : InventoryItemSlot
{
    public override void OnDrop(PointerEventData eventData)
    {
        InventoryItemSlot from;
        if ((from = eventData.pointerDrag.GetComponent<InventoryItemSlot>()))
        {
            if (from.CurrentItem.GetType() == typeof(Weapon))
                equipmentManager.EquipWeapon(from.CurrentItem as Weapon, from.index);
        }
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Right)
            return;

        InventoryItemSlot from;
        if ((from = eventData.pointerDrag.GetComponent<InventoryItemSlot>()))
        {
            if (inventory.inventoryItems.FirstNullIndexAt().HasValue)
            {
                if (from.CurrentItem.GetType() == typeof(Weapon))
                    equipmentManager.UnequipItem(from.CurrentItem as Weapon, from.index);
            }
            else
                equipmentManager.DropEquippedItem(from.CurrentItem as Item);
        }
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

