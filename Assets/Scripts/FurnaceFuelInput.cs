using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FurnaceFuelInput : ItemSlot
{
    protected Furnace furnace;

    public delegate void OnItemUsed();
    public OnItemUsed OnItemUsedCallback;

    public virtual void Initialize(Furnace furnace)
    {
        this.furnace = furnace;
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        //We only want draggin on left mousebutton
        if (eventData.button != PointerEventData.InputButton.Left || currentItem == null)
            return;

        initialParentTransform = transform.parent;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
        transform.SetParent(transform.parent.parent.parent);
        furnace.FuelItem = null;
    }

    public override void OnDrop(PointerEventData eventData)
    {
        if(eventData.pointerDrag.GetComponent<ItemSlot>().CurrentItem.BurningTime > 0 && furnace.FuelItem == null)
        {
            var from = eventData.pointerDrag.GetComponent<InventoryItemSlot>();
            CurrentItem = eventData.pointerDrag.GetComponent<ItemSlot>().CurrentItem;
            PlayerNetwork.LocalPlayer.GetComponent<Inventory>().RemoveItemAtIndex(from.index);
            furnace.FuelItem = currentItem;
        }
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        //We only want draggin on left mousebutton
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
        transform.SetParent(initialParentTransform);
        transform.localPosition = Vector3.zero;
        furnace.FuelItem = currentItem;

        if (EventSystem.current.IsPointerOverGameObject())
            return;

        ItemFactory.CreateWorldObject(PlayerNetwork.LocalPlayer.transform.position, currentItem.Id, currentItem.StackSize);
        furnace.FuelItem = null;
        CurrentItem = null;
    }
}
