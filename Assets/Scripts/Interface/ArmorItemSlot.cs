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
        //Just to override the base method.
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

