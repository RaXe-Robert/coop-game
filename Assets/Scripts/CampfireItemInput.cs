using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

public class CampfireItemInput : ItemSlot
{
    protected Campfire campfire;

    public virtual void Initialize(Campfire campfire)
    {
        this.campfire = campfire;
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
        campfire.InputItem = null;
    }

    public override void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag.GetComponent<ItemSlot>().CurrentItem.CookingResult != null && campfire.InputItem == null)
        {
            var from = eventData.pointerDrag.GetComponent<InventoryItemSlot>();
            CurrentItem = eventData.pointerDrag.GetComponent<ItemSlot>().CurrentItem;
            PlayerNetwork.LocalPlayer.GetComponent<Inventory>().RemoveItemAtIndex(from.index);
            campfire.InputItem = currentItem;
        }
        else
            FeedUI.Instance.AddFeedItem("This item cannot be cooked!", feedType: FeedItem.Type.Fail);
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
        campfire.InputItem = currentItem;

        if (EventSystem.current.IsPointerOverGameObject())
            return;

        ItemFactory.CreateWorldObject(PlayerNetwork.LocalPlayer.transform.position, currentItem.Id, currentItem.StackSize);
        campfire.InputItem = null;
        CurrentItem = null;
    }
}