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
        //Check what gets dropped on this.
        if (!(@fromI = eventData.pointerDrag.GetComponent<InventoryItemSlot>()))
            return;

        //We got an item from our equipment.
        if (@fromI.index == -1 && CurrentItem != null)
        {
            //We cant equip a non equippable item.
            if (CurrentItem is Equippable)
                return;

            //We cant swap the items it they arent the same type
            if (@fromI.CurrentItem.GetType() != CurrentItem.GetType())
                return;

            //Check if our item is an Armor and see if it's the same type of armor, if so we can swap the items around.
            if ((@fromI.CurrentItem.GetType() == typeof(Armor) && ((Armor)@fromI.CurrentItem).ArmorType == ((Armor)CurrentItem).ArmorType))
                equipmentManager.EquipArmor(CurrentItem as Armor, index);

            //Check if our item is a Tool and see if it's the same type of tool, if so we can swap the items around.
            else if ((@fromI.CurrentItem.GetType() == typeof(Tool) && ((Tool)@fromI.CurrentItem).ToolType == ((Tool)CurrentItem).ToolType))
                equipmentManager.EquipTool(CurrentItem as Tool, index);

            //Check if we both have weapons if so we can swap them around.
            else if ((@fromI.CurrentItem.GetType() == typeof(Weapon) && CurrentItem.GetType() == typeof(Weapon)))
                equipmentManager.EquipWeapon(CurrentItem as Weapon, index);
        }
        else if (@fromI.index == -1 && CurrentItem == null)
        {
            //We are dragging an equipment piece on an empty inventory slot.
            equipmentManager.UnequipItem(@fromI.CurrentItem as Item, index);
        }

        else
            chest.SwapItems(index, @fromI.index);

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
