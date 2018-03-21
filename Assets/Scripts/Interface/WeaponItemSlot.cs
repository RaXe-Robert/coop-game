using UnityEngine;
using System;
using UnityEngine.EventSystems;

public class WeaponItemSlot : InventoryItemSlot
{
    [SerializeField] private WeaponType slotType;

    public override void OnDrop(PointerEventData eventData)
    {
        InventoryItemSlot from;
        if ((from = eventData.pointerDrag.GetComponent<InventoryItemSlot>()))
        {
            if (from.Item.GetType() == typeof(Weapon))
                equipmentManager.EquipWeapon(from.Item as Weapon);
        }
    }
}

