using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemInput : ItemSlot
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
        if (eventData.pointerDrag.GetComponent<ItemSlot>().CurrentItem.MeltingResult != null)
        {
            var from = eventData.pointerDrag.GetComponent<InventoryItemSlot>();
            CurrentItem = eventData.pointerDrag.GetComponent<ItemSlot>().CurrentItem;
            PlayerNetwork.LocalPlayer.GetComponent<Inventory>().RemoveItemAtIndex(from.index);
        }
    }

    public Item TakeItem()
    {
        if (CurrentItem == null)
            return null;
        else
        {
            if (CurrentItem.StackSize > 1)
                CurrentItem.StackSize--;
            else
                CurrentItem = null;

            OnItemUsedCallback?.Invoke();
            return ItemFactory.CreateNewItem(CurrentItem.Id);
        }
    }
}