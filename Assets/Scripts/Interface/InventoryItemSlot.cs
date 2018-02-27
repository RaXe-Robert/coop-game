using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryItemSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    public Image icon;
    private Item item;

    public Item Item
    {
        get
        {
            return item;
        }
        
        set
        {
            item = value;
            icon.sprite = item.Sprite;
            icon.enabled = true;
        }
    }

    public void Start()
    {
        
    }

    public void Clear()
    {
        item = null;
        icon.sprite = null;
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        if (item == null)
            return;

        var tooltip = Tooltip.Instance;
        tooltip.Show(item.Description);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (item == null)
            return;

        var tooltip = Tooltip.Instance;
        tooltip.Hide();
    }
}
