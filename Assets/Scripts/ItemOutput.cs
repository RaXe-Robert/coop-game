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
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
        transform.SetParent(initialParentTransform);
        transform.localPosition = Vector3.zero;
        furnace.OutputItem = currentItem;
    }
}