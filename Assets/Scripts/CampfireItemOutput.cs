using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

public class CampfireItemOutput : ItemSlot
{
    protected Campfire campfire;

    public delegate void OnItemUsed();

    public virtual void Initialize(Campfire campfire)
    {
        this.campfire = campfire;
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
        campfire.OutputItem = currentItem;

        if (EventSystem.current.IsPointerOverGameObject())
            return;

        ItemFactory.CreateWorldObject(PlayerNetwork.LocalPlayer.transform.position, currentItem.Id, currentItem.StackSize);
        campfire.OutputItem = null;
        CurrentItem = null;
    }
}