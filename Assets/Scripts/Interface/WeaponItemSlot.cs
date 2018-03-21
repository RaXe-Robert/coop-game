using UnityEngine;
using System;
using UnityEngine.EventSystems;

public class WeaponItemSlot : InventoryItemSlot
{
    [SerializeField] private WeaponType slotType;

    public override void OnPointerClick(PointerEventData eventData)
    {
        //We clicked an equipment slot... What now ?
        base.OnPointerClick(eventData);
    }

    public override void OnDrop(PointerEventData eventData)
    {
        base.OnDrop(eventData);
    }
}

