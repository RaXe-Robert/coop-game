using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryItemSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDragHandler, IBeginDragHandler, IEndDragHandler, IDropHandler {
    [SerializeField] public InventoryUI inventoryUI;
    [SerializeField] private Image image;
    
    private Item item;
    private Inventory inventory;
    private CanvasGroup canvasGroup;
    private Transform initialParentTransform;
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
        }
    }

    public void Initialize(int index, Inventory inventory, InventoryUI ui)
    {
        this.index = index;
        this.inventory = inventory;
        this.inventoryUI = ui;
    }

    public void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
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

        Tooltip.Instance.Show(item.Description);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Tooltip.Instance.Hide();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (item == null)
        {
            eventData.pointerDrag = null;
            return;
        }

        initialParentTransform = transform.parent;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
        transform.SetParent(transform.parent.parent.parent);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (item == null)
            return;

        transform.position = eventData.position;
    }

    public void OnDrop(PointerEventData eventData)
    {
        var from = eventData.pointerDrag.GetComponent<InventoryItemSlot>();
        inventory.SwapItem(index, from.index);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
        transform.SetParent(initialParentTransform);
        transform.localPosition  = Vector3.zero;

        if (!EventSystem.current.IsPointerOverGameObject())
        {
            inventory.RemoveItem(index);
        }
    }
}
