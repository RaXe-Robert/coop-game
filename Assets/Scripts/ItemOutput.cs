using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.EventSystems;

public class ItemOutput : ItemSlot
{
    public delegate void OnItemUsed();
    public OnItemUsed OnItemUsedCallback;

    protected override void Start()
    {
        base.Start();
        OnItemUsedCallback += UpdateUI;
    }

    public override void OnDrop(PointerEventData eventData)
    {
        //This shouldn't be allowed this slot is only used for output
        return;
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