using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.EventSystems;

public class ItemOutput : ItemSlot
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

    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);
    }

    public override void OnDrag(PointerEventData eventData)
    {
        base.OnDrag(eventData);
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);
    }

    public override void OnDrop(PointerEventData eventData)
    {
        //This shouldn't be allowed this slot is only used for output
        return;
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
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

    public void DepositItem(Item itemToDeposit)
    {
        if (CurrentItem != null)
        {
            if (itemToDeposit.Id != CurrentItem?.Id)
                return;
            CurrentItem.StackSize += itemToDeposit.StackSize;
        }
        else CurrentItem = itemToDeposit;

        OnItemUsedCallback?.Invoke();
    }
}