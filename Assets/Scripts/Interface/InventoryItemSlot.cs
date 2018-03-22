using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryItemSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IDragHandler, IBeginDragHandler, IEndDragHandler, IDropHandler {
    [SerializeField] protected Image image;
    [SerializeField] private Text stackSizeText;
    [SerializeField] private Image textBackground;
    
    protected ItemBase item;
    protected Inventory inventory;
    protected EquipmentManager equipmentManager;
    protected CanvasGroup canvasGroup;
    protected Transform initialParentTransform;
    private int index;

    public ItemBase Item
    {
        get
        {
            return item;
        }
        
        set
        {
            image.enabled = true;
            item = value;
            image.sprite = item?.Sprite;
            if (item?.StackSize > 1)
            {
                stackSizeText.text = item.StackSize.ToString();
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
        item = null;
        image.sprite = null;
        image.enabled = false;
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

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        if (item == null)
            return;

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (item.IsConsumable && item.OnConsumedEffects != null && item.OnConsumedEffects.Count > 0)
            {
                PlayerNetwork.PlayerObject.GetComponent<StatusEffectComponent>().AddStatusEffect(item.OnConsumedEffects);
                inventory.RemoveItem(index);
                Tooltip.Instance.Hide();
            }

            else if(item.Equippable)
            {
                equipmentManager.EquipItem(item);
                Tooltip.Instance.Hide();
            }
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

    public virtual void OnDrop(PointerEventData eventData)
    {
        InventoryItemSlot from;
        //Check what gets dropped on this.
        if((from = eventData.pointerDrag.GetComponent<InventoryItemSlot>()))
        {
            //We got an item from our equipment.
            if(from.index == -1 && Item != null)
            {
                //We cant equip a non equippable item.
                if (!item.Equippable)
                    return;

                //We cant swap the items it they arent the same type
                if (from.Item.GetType() == Item.GetType())
                    return;

                //Check if our item is an Armor and see if it's the same type of armor, if so we can swap the items around.
                if ((from.Item.GetType() == typeof(Armor) && ((Armor)from.Item).ArmorType == ((Armor)Item).ArmorType))
                    equipmentManager.EquipArmor(Item as Armor);

                //Check if our item is a Tool and see if it's the same type of tool, if so we can swap the items around.
                else if ((from.Item.GetType() == typeof(Tool) && ((Tool)from.Item).ToolType == ((Tool)Item).ToolType))
                    equipmentManager.EquipTool(Item as Tool);

                //Check if we both have weapons if so we can swap them around.
                else if ((from.Item.GetType() == typeof(Weapon) && Item.GetType() == typeof(Weapon)))
                    equipmentManager.EquipWeapon(Item as Weapon);
            }
            else if(from.index == -1 && Item == null)
            {
                //We are dragging an equipment piece on an empty inventory slot.
                equipmentManager.UnEquipItem(from.Item, index);
            }

            else 
                inventory.SwapItem(index, from.index);
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
            ItemFactory.CreateWorldObject(PlayerNetwork.PlayerObject.transform.position, item.Id, item.StackSize);
            inventory.RemoveItem(index);
        }
    }
}
