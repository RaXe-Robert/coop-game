using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FuelInput : ItemSlot {

    //When an item gets dropped on this slot, it goes to this buildable
    [SerializeField] private IFuelInput buildable;

    private Item currentItem;
    public Item CurrentItem
    {
        get
        {
            return currentItem;
        }

        set
        {
            throw new System.NotImplementedException();
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }

    public void OnDrag(PointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        if (eventData.pointerDrag.GetComponent<ItemSlot>() != null)
        {
            Debug.Log(eventData.pointerDrag.GetComponent<InventoryItemSlot>().CurrentItem);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }
}
