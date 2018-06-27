using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FuelInput : ItemSlot
{
    protected Furnace furnace;

    public delegate void OnItemUsed();
    public OnItemUsed OnItemUsedCallback;
    private Sprite initialImage;

    public virtual void Initialize(Furnace furnace)
    {
        this.furnace = furnace;
        initialImage = image.sprite;
    }

    public override void OnDrop(PointerEventData eventData)
    {
        if(eventData.pointerDrag.GetComponent<ItemSlot>().CurrentItem.BurningTime > 0)
        {
            var from = eventData.pointerDrag.GetComponent<InventoryItemSlot>();
            CurrentItem = eventData.pointerDrag.GetComponent<ItemSlot>().CurrentItem;
            PlayerNetwork.LocalPlayer.GetComponent<Inventory>().RemoveItemAtIndex(from.index);
            furnace.FuelItem = currentItem;
        }
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
        transform.SetParent(initialParentTransform);
        transform.localPosition = Vector3.zero;
        furnace.FuelItem = currentItem;
    }
}
