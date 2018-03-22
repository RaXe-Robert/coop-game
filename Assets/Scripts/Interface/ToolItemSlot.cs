using UnityEngine;
using System;
using UnityEngine.EventSystems;

public class ToolItemSlot : InventoryItemSlot
{
    [SerializeField] private ToolType slotType;
    public ToolType SlotType { get { return slotType; } }

    public override void OnDrop(PointerEventData eventData)
    {
        InventoryItemSlot from;
        if((from = eventData.pointerDrag.GetComponent<InventoryItemSlot>()))
        {
            if (from.Item.GetType() == typeof(Tool))
                equipmentManager.EquipTool(from.Item as Tool);
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
        {
            ItemFactory.CreateWorldObject(PlayerNetwork.PlayerObject.transform.position, item.Id, item.StackSize);
            equipmentManager.DropEquippedItem(Item);
        }
    }
}

