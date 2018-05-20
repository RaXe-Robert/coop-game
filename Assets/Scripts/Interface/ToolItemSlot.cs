using UnityEngine;
using System;
using UnityEngine.EventSystems;
using Assets.Scripts.Utilities;

public class ToolItemSlot : InventoryItemSlot
{
    [SerializeField] private ToolType slotType;
    public ToolType SlotType { get { return slotType; } }

    public override void OnDrop(PointerEventData eventData)
    {
        InventoryItemSlot from;
        if((from = eventData.pointerDrag.GetComponent<InventoryItemSlot>()))
        {
            if (from.CurrentItem.GetType() == typeof(Tool))
                equipmentManager.EquipTool(from.CurrentItem as Tool, from.index);
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
                if (from.CurrentItem.GetType() == typeof(Tool))
                    equipmentManager.UnequipItem(from.CurrentItem as Tool, from.index);
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

