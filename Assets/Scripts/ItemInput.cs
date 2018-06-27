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
    private Sprite initialImage;

    public virtual void Initialize(Furnace furnace)
    {
        this.furnace = furnace;
        initialImage = image.sprite;
    }

    public override void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag.GetComponent<ItemSlot>().CurrentItem.MeltingResult != null)
        {
            var from = eventData.pointerDrag.GetComponent<InventoryItemSlot>();
            CurrentItem = eventData.pointerDrag.GetComponent<ItemSlot>().CurrentItem;
            PlayerNetwork.LocalPlayer.GetComponent<Inventory>().RemoveItemAtIndex(from.index);
            furnace.InputItem = currentItem;
        }
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
        transform.SetParent(initialParentTransform);
        transform.localPosition = Vector3.zero;
        furnace.InputItem = currentItem;
    }
}