using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FuelInput : ItemSlot
{
    protected Furnace furnace;

    public delegate void OnItemUsed();
    public OnItemUsed OnItemUsedCallback;

    public virtual void Initialize(Furnace furnace)
    {
        this.furnace = furnace;
    }

    protected override void Start()
    {
        base.Start();
        OnItemUsedCallback += UpdateUI;
    }

    public override void OnDrop(PointerEventData eventData)
    {
        if(eventData.pointerDrag.GetComponent<ItemSlot>().CurrentItem.BurningTime > 0)
        {
            var from = eventData.pointerDrag.GetComponent<InventoryItemSlot>();
            CurrentItem = eventData.pointerDrag.GetComponent<ItemSlot>().CurrentItem;
            PlayerNetwork.LocalPlayer.GetComponent<Inventory>().RemoveItemAtIndex(from.index);
        }
    }

    public void TakeFuel()
    {
        if (CurrentItem == null)
            return;
        else
        {
            if(CurrentItem.StackSize > 1)
                CurrentItem.StackSize--;
            else
                CurrentItem = null;

            OnItemUsedCallback?.Invoke();
        }
    }
}
