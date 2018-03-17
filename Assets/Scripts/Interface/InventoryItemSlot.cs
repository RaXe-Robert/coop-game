using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryItemSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IDragHandler, IBeginDragHandler, IEndDragHandler, IDropHandler {
    [SerializeField] public InventoryUI inventoryUI;
    [SerializeField] private Image image;
    [SerializeField] private Text stackSizeText;
    [SerializeField] private Image textBackground;
    
    private ItemBase item;
    private Inventory inventory;
    private CanvasGroup canvasGroup;
    private Transform initialParentTransform;
    private int index;

    public ItemBase Item
    {
        get
        {
            return item;
        }
        
        set
        {
            item = value;
            image.sprite = item?.Sprite;
            if (item?.GetType() == typeof(Resource))
            {
                stackSizeText.text = ((Resource)item).Amount.ToString();
                textBackground.enabled = true;
            }
            else
            {
                textBackground.enabled = false;
                stackSizeText.text = "";
            }
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
    
    public void OnPointerClick(PointerEventData eventData)
    {
        if (item == null)
            return;

        switch (eventData.button)
        {
            case PointerEventData.InputButton.Left:
                if (item.IsConsumable && item.OnConsumedEffects != null && item.OnConsumedEffects.Count > 0)
                {
                    PlayerNetwork.PlayerObject.GetComponent<StatusEffectComponent>().AddStatusEffect(item.OnConsumedEffects);
                    inventory.RemoveItemById(item.Id, 1);
                    Tooltip.Instance.Hide();
                }
                break;
            case PointerEventData.InputButton.Right:
                break;
            case PointerEventData.InputButton.Middle:
                break;
        }
        
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
        InventoryItemSlot from;
        //Check what gets dropped on this.
        if((from = eventData.pointerDrag.GetComponent<InventoryItemSlot>()))
        {
            inventory.SwapItem(index, from.index);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
        transform.SetParent(initialParentTransform);
        transform.localPosition  = Vector3.zero;

        if (!EventSystem.current.IsPointerOverGameObject())
        {
            ItemFactory.CreateWorldObject(PlayerNetwork.PlayerObject.transform.position, item.Id, (item.GetType() == typeof(Resource) ? ((Resource)item).Amount : 1));
            inventory.RemoveItem(index);
        }
    }
}
