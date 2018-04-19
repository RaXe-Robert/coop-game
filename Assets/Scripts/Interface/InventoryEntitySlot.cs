using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryEntitySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IDragHandler, IBeginDragHandler, IEndDragHandler, IDropHandler {
    [SerializeField] protected Image image;
    [SerializeField] private Text stackSizeText;
    [SerializeField] private Image textBackground;

    protected internal int index;
    protected Inventory inventory;
    protected EquipmentManager equipmentManager;

    protected EntityBase entity;

    protected CanvasGroup canvasGroup;
    protected Transform initialParentTransform;

    public EntityBase Entity
    {
        get
        {
            return entity;
        }
        
        set
        {
            entity = value;
            image.sprite = entity?.Sprite;
            if (entity?.StackSize > 1)
            {
                stackSizeText.text = entity.StackSize.ToString();
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
        entity = null;
        image.sprite = null;
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        if (entity == null)
            return;

        Tooltip.Instance.Show(entity.Name, entity.Description);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Tooltip.Instance.Hide();
    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        if (entity == null)
            return;

        if (eventData.button == PointerEventData.InputButton.Right)
        {

            if (entity is BuildableBase)
            {
                inventory.BuildingController?.ActivateBuildMode(entity as BuildableBase);
            }
            else if (entity is ItemBase)
            {
                ItemBase item = entity as ItemBase;
                if (item.IsConsumable && item.OnConsumedEffects != null && item.OnConsumedEffects.Count > 0)
                {
                    PlayerNetwork.PlayerObject.GetComponent<StatusEffectComponent>().AddStatusEffect(item.OnConsumedEffects);

                    if (item.StackSize > 1)
                    {
                        item.StackSize--;
                        inventory.OnEntityChangedCallback?.Invoke();
                    }
                    else
                        inventory.RemoveEntityAtIndex(index);

                    Tooltip.Instance.Hide();
                }

                else if (item.Equippable)
                {
                    equipmentManager.EquipItem(item, index);
                    Tooltip.Instance.Hide();
                }
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (entity == null)
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
        if (entity == null)
            return;

        transform.position = eventData.position;
    }

    public virtual void OnDrop(PointerEventData eventData)
    {
        InventoryEntitySlot from;
        //Check what gets dropped on this.
        if((from = eventData.pointerDrag.GetComponent<InventoryEntitySlot>()))
        {
            //We got an item from our equipment.
            if(from.index == -1 && Entity != null)
            {
                ItemBase item = entity as ItemBase;
                //We cant equip a non equippable item.
                if (!item.Equippable)
                    return;

                //We cant swap the items it they arent the same type
                if (from.Entity.GetType() != Entity.GetType())
                    return;

                //Check if our item is an Armor and see if it's the same type of armor, if so we can swap the items around.
                if ((from.Entity.GetType() == typeof(Armor) && ((Armor)from.Entity).ArmorType == ((Armor)Entity).ArmorType))
                    equipmentManager.EquipArmor(Entity as Armor, index);

                //Check if our item is a Tool and see if it's the same type of tool, if so we can swap the items around.
                else if ((from.Entity.GetType() == typeof(Tool) && ((Tool)from.Entity).ToolType == ((Tool)Entity).ToolType))
                    equipmentManager.EquipTool(Entity as Tool, index);

                //Check if we both have weapons if so we can swap them around.
                else if ((from.Entity.GetType() == typeof(Weapon) && Entity.GetType() == typeof(Weapon)))
                    equipmentManager.EquipWeapon(Entity as Weapon, index);
            }
            else if(from.index == -1 && Entity == null)
            {
                //We are dragging an equipment piece on an empty inventory slot.
                equipmentManager.UnequipItem(from.Entity as ItemBase, index);
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
            EntityFactory.CreateWorldObject(PlayerNetwork.PlayerObject.transform.position, entity.Id, entity.StackSize);
            inventory.RemoveEntityAtIndex(index);
        }
    }
}
