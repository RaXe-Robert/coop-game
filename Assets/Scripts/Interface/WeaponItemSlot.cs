using UnityEngine;
using System;
using UnityEngine.EventSystems;

public class WeaponItemSlot : InventoryEntitySlot
{
    public override void OnDrop(PointerEventData eventData)
    {
        InventoryEntitySlot from;
        if ((from = eventData.pointerDrag.GetComponent<InventoryEntitySlot>()))
        {
            if (from.Entity.GetType() == typeof(Weapon))
                equipmentManager.EquipWeapon(from.Entity as Weapon, from.index);
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

