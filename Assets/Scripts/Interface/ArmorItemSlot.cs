﻿using UnityEngine;
using System;
using UnityEngine.EventSystems;

public class ArmorItemSlot : InventoryItemSlot
{
    [SerializeField] private ArmorType slotType;

    public override void OnDrop(PointerEventData eventData)
    {
        InventoryItemSlot from;
        if ((from = eventData.pointerDrag.GetComponent<InventoryItemSlot>()))
        {
            if (from.Item.GetType() == typeof(Armor))
                equipmentManager.EquipArmor(from.Item as Armor);
        }
    }
}

