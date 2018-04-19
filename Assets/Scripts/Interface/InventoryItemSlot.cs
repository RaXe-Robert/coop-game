﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryItemSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IDragHandler, IBeginDragHandler, IEndDragHandler, IDropHandler {
    [SerializeField] protected Image image;
    [SerializeField] private Text stackSizeText;
    [SerializeField] private Image textBackground;

    protected internal int index;
    protected Inventory inventory;
    protected EquipmentManager equipmentManager;

    protected Item currentItem;

    protected CanvasGroup canvasGroup;
    protected Transform initialParentTransform;

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

    public virtual void Initialize(int index, Inventory inventory, EquipmentManager equipmentManager)
    {
        this.index = index;
        this.inventory = inventory;
        this.equipmentManager = equipmentManager;
    }

    public void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Clear()
    {
        currentItem = null;
        image.sprite = null;
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
        if (CurrentItem == null)
            return;

        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (currentItem is BuildableBase)
            {
                inventory.BuildingController?.ActivateBuildMode(currentItem as BuildableBase);
            }
            else if (CurrentItem is Item)
            {
                Consumable item = currentItem as Consumable;
                if (item.IsConsumable && item.OnConsumedEffects != null && item.OnConsumedEffects.Count > 0)
                {
                    PlayerNetwork.PlayerObject.GetComponent<StatusEffectComponent>().AddStatusEffect(item.OnConsumedEffects);

                    if (item.StackSize > 1)
                    {
                        item.StackSize--;
                        inventory.OnItemChangedCallback?.Invoke();
                    }
                    else
                        inventory.RemoveItemAtIndex(index);

                    Tooltip.Instance.Hide();
                }

            }
            else if (CurrentItem is Equippable)
            {
                equipmentManager.EquipItem(CurrentItem, index);
                Tooltip.Instance.Hide();
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
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
        if (currentItem == null)
            return;

        transform.position = eventData.position;
    }

    public virtual void OnDrop(PointerEventData eventData)
    {
        InventoryItemSlot from;
        //Check what gets dropped on this.
        if((from = eventData.pointerDrag.GetComponent<InventoryItemSlot>()))
        {
            //We got an item from our equipment.
            if(from.index == -1 && CurrentItem != null)
            {
                //We cant equip a non equippable item.
                if (CurrentItem is Equippable)
                    return;

                //We cant swap the items it they arent the same type
                if (from.CurrentItem.GetType() != CurrentItem.GetType())
                    return;

                //Check if our item is an Armor and see if it's the same type of armor, if so we can swap the items around.
                if ((from.CurrentItem.GetType() == typeof(Armor) && ((Armor)from.CurrentItem).ArmorType == ((Armor)CurrentItem).ArmorType))
                    equipmentManager.EquipArmor(CurrentItem as Armor, index);

                //Check if our item is a Tool and see if it's the same type of tool, if so we can swap the items around.
                else if ((from.CurrentItem.GetType() == typeof(Tool) && ((Tool)from.CurrentItem).ToolType == ((Tool)CurrentItem).ToolType))
                    equipmentManager.EquipTool(CurrentItem as Tool, index);

                //Check if we both have weapons if so we can swap them around.
                else if ((from.CurrentItem.GetType() == typeof(Weapon) && CurrentItem.GetType() == typeof(Weapon)))
                    equipmentManager.EquipWeapon(CurrentItem as Weapon, index);
            }
            else if(from.index == -1 && CurrentItem == null)
            {
                //We are dragging an equipment piece on an empty inventory slot.
                equipmentManager.UnequipItem(from.CurrentItem as Item, index);
            }

            else 
                inventory.SwapEntities(index, from.index);
        }
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
        transform.SetParent(initialParentTransform);
        transform.localPosition  = Vector3.zero;

        if (!EventSystem.current.IsPointerOverGameObject())
        {
            ItemFactory.CreateWorldObject(PlayerNetwork.PlayerObject.transform.position, currentItem.Id, currentItem.StackSize);
            inventory.RemoveItemAtIndex(index);
        }
    }
}
