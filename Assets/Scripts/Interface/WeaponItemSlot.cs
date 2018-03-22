using UnityEngine;
using System;
using UnityEngine.EventSystems;

public class WeaponItemSlot : InventoryItemSlot
{
    public override void OnDrop(PointerEventData eventData)
    {
        InventoryItemSlot from;
        if ((from = eventData.pointerDrag.GetComponent<InventoryItemSlot>()))
        {
            if (from.Item.GetType() == typeof(Weapon))
                equipmentManager.EquipWeapon(from.Item as Weapon);
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

