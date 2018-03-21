using UnityEngine;
using System;
using UnityEngine.EventSystems;

public class ToolItemSlot : InventoryItemSlot
{
    [SerializeField] private ToolType slotType;

    public override void OnDrop(PointerEventData eventData)
    {
        InventoryItemSlot from;
        if((from = eventData.pointerDrag.GetComponent<InventoryItemSlot>()))
        {
            if (from.Item.GetType() == typeof(Tool))
                equipmentManager.EquipTool(from.Item as Tool);
        }
    }
}

