﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryItemSlot : ItemSlot {
    protected internal int index;
    protected Inventory inventory;
    protected EquipmentManager equipmentManager;

    public virtual void Initialize(int index, Inventory inventory, EquipmentManager equipmentManager)
    {
        this.index = index;
        this.inventory = inventory;
        this.equipmentManager = equipmentManager;
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Right)
            return;

        var @base = currentItem as BuildableBase;
        if (@base != null && inventory != null)
            inventory.BuildingController.ActivateBuildMode(@base);
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
                if (inventory != null)
                    inventory.OnItemChangedCallback?.Invoke();
            }
            else if (inventory != null) 
                inventory.RemoveItemAtIndex(index);

            Tooltip.Instance.Hide();
        }
    }

    public override void OnDrop(PointerEventData eventData)
    {
        //We only want draggin on left mousebutton
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        ChestItemSlot fromChest;
        if ((fromChest = eventData.pointerDrag.GetComponent<ChestItemSlot>()) != null)
        {
            inventory.AddItemAtIndex(fromChest.CurrentItem.Id, index, fromChest.CurrentItem.StackSize);

            Chest chestReference = ChestUI.Instance.chest;            
            if (chestReference != null)
                chestReference.RemoveItemAtIndex(fromChest.index);
        }

        FurnaceItemOutput fromFurnaceOutput;
        if((fromFurnaceOutput = eventData.pointerDrag.GetComponent<FurnaceItemOutput>()) != null)
        {
            inventory.AddItemAtIndex(fromFurnaceOutput.CurrentItem.Id, index, fromFurnaceOutput.CurrentItem.StackSize);
            fromFurnaceOutput.CurrentItem = null;
        }

        FurnaceItemInput fromFurnaceInput;
        if ((fromFurnaceInput = eventData.pointerDrag.GetComponent<FurnaceItemInput>()) != null)
        {
            inventory.AddItemAtIndex(fromFurnaceInput.CurrentItem.Id, index, fromFurnaceInput.CurrentItem.StackSize);
            fromFurnaceInput.CurrentItem = null;
        }

        FurnaceFuelInput fromFurnaceFuel;
        if ((fromFurnaceFuel = eventData.pointerDrag.GetComponent<FurnaceFuelInput>()) != null)
        {
            inventory.AddItemAtIndex(fromFurnaceFuel.CurrentItem.Id, index, fromFurnaceFuel.CurrentItem.StackSize);
            fromFurnaceFuel.CurrentItem = null;
        }

        CampfireItemOutput fromCampfireOutput;
        if ((fromCampfireOutput = eventData.pointerDrag.GetComponent<CampfireItemOutput>()) != null)
        {
            inventory.AddItemAtIndex(fromCampfireOutput.CurrentItem.Id, index, fromCampfireOutput.CurrentItem.StackSize);
            fromCampfireOutput.CurrentItem = null;
        }

        CampfireItemInput fromCampfireInput;
        if ((fromCampfireInput = eventData.pointerDrag.GetComponent<CampfireItemInput>()) != null)
        {
            inventory.AddItemAtIndex(fromCampfireInput.CurrentItem.Id, index, fromCampfireInput.CurrentItem.StackSize);
            fromCampfireInput.CurrentItem = null;
        }

        CampfireFuelInput fromCampfireFuel;
        if ((fromCampfireFuel = eventData.pointerDrag.GetComponent<CampfireFuelInput>()) != null)
        {
            inventory.AddItemAtIndex(fromCampfireFuel.CurrentItem.Id, index, fromCampfireFuel.CurrentItem.StackSize);
            fromCampfireFuel.CurrentItem = null;
        }

        InventoryItemSlot from;
        //Check what gets dropped on this.
        if (!(@from = eventData.pointerDrag.GetComponent<ItemSlot>() as InventoryItemSlot))
            return;
        
        //We got an item from our equipment.
        if(@from.index == -1 && CurrentItem != null)
        {
            //We cant swap the items it they arent the same type
            if (@from.CurrentItem.GetType() != CurrentItem.GetType())
                return;

            //Check if our item is an Armor and see if it's the same type of armor, if so we can swap the items around.
            if ((@from.CurrentItem.GetType() == typeof(Armor) && ((Armor)@from.CurrentItem).ArmorType == ((Armor)CurrentItem).ArmorType))
                equipmentManager.EquipArmor(CurrentItem as Armor, index);
        }
        else if(@from.index == -1 && CurrentItem == null)
        {
            //We are dragging an equipment piece on an empty inventory slot.
            equipmentManager.UnequipArmor(@from.CurrentItem as Item, index);
        }
        else 
            inventory.SwapItems(index, @from.index);
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
        inventory.RemoveItemAtIndex(index);
    }
}
