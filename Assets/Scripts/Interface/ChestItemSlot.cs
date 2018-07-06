using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ChestItemSlot : ItemSlot {
    protected internal int index;
    protected Chest chest;
    protected EquipmentManager equipmentManager;

    public virtual void Initialize(int index, Chest chest, EquipmentManager equipmentManager)
    {
        this.index = index;
        this.chest = chest;
        this.equipmentManager = equipmentManager;
        
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Right)
            return;
        var @base = currentItem as BuildableBase;
        if (@base != null && chest != null)
            chest.BuildingController.ActivateBuildMode(@base);
        else if (CurrentItem is Armor)
        {
            equipmentManager.EquipArmor(CurrentItem as Armor, index);
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

    public override void OnDrop(PointerEventData eventData)
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

    public override void OnEndDrag(PointerEventData eventData)
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
