using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemOutput : ItemSlot
{
    protected Furnace furnace;

    public delegate void OnItemUsed();
    private Sprite initialImage;

    public virtual void Initialize(Furnace furnace)
    {
        this.furnace = furnace;
        initialImage = image.sprite;
    }

    public override void OnDrop(PointerEventData eventData)
    {
        //This shouldn't be allowed this slot is only used for output
        return;
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
        furnace.OutputItem = currentItem;

        if (EventSystem.current.IsPointerOverGameObject())
            return;

        ItemFactory.CreateWorldObject(PlayerNetwork.LocalPlayer.transform.position, currentItem.Id, currentItem.StackSize);
        furnace.OutputItem = null;
        CurrentItem = null;
    }
}