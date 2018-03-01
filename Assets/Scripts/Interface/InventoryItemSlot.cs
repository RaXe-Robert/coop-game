using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryItemSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDragHandler, IBeginDragHandler, IEndDragHandler, IDropHandler {
    [SerializeField] public InventoryUI inventoryUI;
    [SerializeField] private Image image;
    
    private Item item;
    private Vector3 initialPosition;
    private Inventory inventory;
    private int index;

    public Item Item
    {
        get
        {
            return item;
        }
        
        set
        {
            item = value;
            image.sprite = item?.Sprite;
            image.enabled = item != null;
        }
    }

    public void Initialize(int index, Inventory inventory)
    {
        this.index = index;
        this.inventory = inventory;
    }

    public void Start()
    {
        initialPosition = transform.position;
        image = GetComponent<Image>();
    }

    public void Clear()
    {
        item = null;
        image.sprite = null;
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

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.position = initialPosition;
        //image.raycastTarget = true;
    }

    public void OnDrop(PointerEventData eventData)
    {
        var from = eventData.pointerDrag.GetComponent<InventoryItemSlot>();
        Item = from.item;
        from.Item = null;
        inventoryUI.UpdateUI();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        image.raycastTarget = false;
    }
}
