using UnityEngine;
using System;
using UnityEngine.EventSystems;

public class ToolItemSlot : InventoryEntitySlot
{
    [SerializeField] private ToolType slotType;
    public ToolType SlotType { get { return slotType; } }

    public override void OnDrop(PointerEventData eventData)
    {
        InventoryEntitySlot from;
        if((from = eventData.pointerDrag.GetComponent<InventoryEntitySlot>()))
        {
            if (from.Entity.GetType() == typeof(Tool))
                equipmentManager.EquipTool(from.Entity as Tool, from.index);
        }
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        //Just to override the base method.
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
        transform.SetParent(initialParentTransform);
        transform.localPosition = Vector3.zero;

        if (!EventSystem.current.IsPointerOverGameObject())
            equipmentManager.DropEquippedItem(Entity as ItemBase);
    }
}

