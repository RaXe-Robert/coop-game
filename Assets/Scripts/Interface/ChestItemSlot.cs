using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ChestItemSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IDragHandler, IBeginDragHandler, IEndDragHandler, IDropHandler {
    [SerializeField] protected Image image;
    [SerializeField] private Text stackSizeText;
    [SerializeField] private Image textBackground;

    protected internal int index;
    protected Chest chest;
    protected EquipmentManager equipmentManager;

    protected Item currentItem;

    protected CanvasGroup canvasGroup;
    protected Transform initialParentTransform;
    
    private Sprite initalImage;

    public Item CurrentItem
    {
        get
        {
            return currentItem;
        }
        
        set
        {
            currentItem = value;
            image.sprite = currentItem?.Sprite;
            if (currentItem?.StackSize > 1)
            {
                stackSizeText.text = currentItem.StackSize.ToString();
                textBackground.enabled = true;
            }
            else
            {
                textBackground.enabled = false;
                stackSizeText.text = "";
            }
        }
    }

    public virtual void Initialize(int index, Chest chest, EquipmentManager equipmentManager)
    {
        this.index = index;
        this.chest = chest;
        this.equipmentManager = equipmentManager;

        initalImage = image.sprite;
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Clear()
    {
        currentItem = null;
        image.sprite = initalImage;
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        if (currentItem == null)
            return;

        Tooltip.Instance.Show(currentItem.Name, currentItem.Description);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Tooltip.Instance.Hide();
    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Right)
            return;
        var @base = currentItem as BuildableBase;
        if (@base != null && chest != null)
            chest.BuildingController.ActivateBuildMode(@base);
        else if (CurrentItem is Armor || CurrentItem is Tool || CurrentItem is Weapon)
        {
            equipmentManager.EquipItem(CurrentItem, index);
            Tooltip.Instance.Hide();
        }
        else
        {
            var item = currentItem as Consumable;
            if (item == null || !item.IsConsumable || item.OnConsumedEffects == null || item.OnConsumedEffects.Count <= 0)
                return;
            
            PlayerNetwork.LocalPlayer.GetComponent<StatusEffectComponent>().AddStatusEffect(item.OnConsumedEffects);

            if (item.StackSize > 1)
            {
                item.StackSize--;
                if (chest != null)
                    chest.OnItemChangedCallback?.Invoke();
            }
            else if (chest != null)
                chest.RemoveItemAtIndex(index);

            Tooltip.Instance.Hide();
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //We only want draggin on left mousebutton
        if (eventData.button != PointerEventData.InputButton.Left)
            return;
        
        if (currentItem == null)
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
        //We only want draggin on left mousebutton
        if (eventData.button != PointerEventData.InputButton.Left)
            return;
        
        if (currentItem == null)
            return;

        transform.position = eventData.position;
    }

    public virtual void OnDrop(PointerEventData eventData)
    {
        //We only want draggin on left mousebutton
        if (eventData.button != PointerEventData.InputButton.Left)
            return;
        
        InventoryItemSlot fromI;
        ChestItemSlot fromChest;
        if ((@fromI = eventData.pointerDrag.GetComponent<InventoryItemSlot>()))
        {
            chest.AddItemAtIndex(@fromI.CurrentItem.Id, index, fromI.CurrentItem.StackSize);

            PlayerNetwork.LocalPlayer.GetComponent<Inventory>().RemoveItemAtIndex(fromI.index);
        }
        else if ((fromChest = eventData.pointerDrag.GetComponent<ChestItemSlot>()))
            chest.SwapItems(index, fromChest.index);

    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        //We only want draggin on left mousebutton
        if (eventData.button != PointerEventData.InputButton.Left)
            return;
        
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
        transform.SetParent(initialParentTransform);
        transform.localPosition  = Vector3.zero;

        if (EventSystem.current.IsPointerOverGameObject())
            return;
        
        ItemFactory.CreateWorldObject(PlayerNetwork.LocalPlayer.transform.position, currentItem.Id, currentItem.StackSize);        
        chest.RemoveItemAtIndex(index);
    }
}
