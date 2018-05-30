using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FuelInput : ItemSlot {

    //When an item gets dropped on this slot, it goes to this buildable
    [SerializeField] private BuildableWorldObject buildable;

    public override void OnDrop(PointerEventData eventData)
    {
        if(eventData.pointerDrag.GetComponent<ItemSlot>().CurrentItem.BurningTime > 0)
        {
            CurrentItem = eventData.pointerDrag.GetComponent<ItemSlot>().CurrentItem;
        }
    }
}
