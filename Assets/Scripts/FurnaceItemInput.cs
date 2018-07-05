using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

public class FurnaceItemInput : ItemSlot
{
    protected Furnace furnace;

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
        furnace.InputItem = null;
    }

    public override void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag.GetComponent<ItemSlot>().CurrentItem.MeltingResult != null && furnace.InputItem == null)
        {
            var from = eventData.pointerDrag.GetComponent<InventoryItemSlot>();
            CurrentItem = eventData.pointerDrag.GetComponent<ItemSlot>().CurrentItem;
            PlayerNetwork.LocalPlayer.GetComponent<Inventory>().RemoveItemAtIndex(from.index);
            furnace.InputItem = currentItem;
        }
        else
            FeedUI.Instance.AddFeedItem("This item cannot be melted!", feedType: FeedItem.Type.Fail);
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
        furnace.InputItem = currentItem;

        if (EventSystem.current.IsPointerOverGameObject())
            return;

        ItemFactory.CreateWorldObject(PlayerNetwork.LocalPlayer.transform.position, currentItem.Id, currentItem.StackSize);
        furnace.InputItem = null;
        CurrentItem = null;
    }
}