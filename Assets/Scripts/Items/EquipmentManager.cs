using System;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    private List<Tool> equippedTools;
    private Weapon equippedWeapon;
    private List<Armor> equippedArmor;

    private Inventory inventory;

    public delegate void OnItemEquipped();
    public OnItemEquipped OnItemEquippedCallBack;

    public bool HasWeaponEquipped { get { return equippedWeapon != null; } }

    private void Start()
    {
        equippedTools = new List<Tool>(2);
        equippedArmor = new List<Armor>(4);
        inventory = FindObjectOfType<Inventory>();
    }

    public void EquipTool(Tool toolToEquip)
    {
        if (HasToolEquipped(toolToEquip.ToolType))
        {
            var currentEquipped = GetEquippedTool(toolToEquip.ToolType);
            inventory.RemoveItemById(toolToEquip.Id);
            equippedTools.Remove(currentEquipped);

            inventory.AddItemById(currentEquipped.Id, 1);
            equippedTools.Add(toolToEquip);
        }
        else
        {
            equippedTools.Add(toolToEquip);
            inventory.RemoveItemById(toolToEquip.Id);
        }

        OnItemEquippedCallBack?.Invoke();
    }

    public void EquipWeapon(Weapon weaponToEquip)
    {
        if (HasWeaponEquipped)
        {
            var currentEquipped = equippedWeapon;
            inventory.RemoveItemById(weaponToEquip.Id);
            equippedWeapon = null;

            equippedWeapon = weaponToEquip;
            inventory.AddItemById(currentEquipped.Id);
        }
        else
        {
            inventory.RemoveItemById(weaponToEquip.Id);
            equippedWeapon = weaponToEquip;
        }

        OnItemEquippedCallBack?.Invoke();
    }

    public void EquipArmor(Armor armorToEquip)
    {
        if (HasArmorEquipped(armorToEquip.ArmorType))
        {
            var currentEquipped = GetEquippedArmorByType(armorToEquip.ArmorType);
            equippedArmor.Remove(currentEquipped);
            inventory.RemoveItemById(armorToEquip.Id);

            inventory.AddItemById(currentEquipped.Id);
            equippedArmor.Add(armorToEquip);
        }
        else
        {
            equippedArmor.Add(armorToEquip);
            inventory.RemoveItemById(armorToEquip.Id);
        }

        OnItemEquippedCallBack?.Invoke();
    }

    public void EquipItem(ItemBase item)
    {
        if (!item.Equippable)
            return;

        if (item.GetType() == typeof(Armor))
            EquipArmor(item as Armor);
        else if (item.GetType() == typeof(Weapon))
            EquipWeapon(item as Weapon);
        else if (item.GetType() == typeof(Tool))
            EquipTool(item as Tool);
    }

    public bool HasToolEquipped(ToolType toolType)
    {
        for (int i = 0; i < equippedTools.Count; i++)
        {
            if (equippedTools[i].ToolType == toolType)
                return true;
        }
        return false;
    }

    public bool HasArmorEquipped(ArmorType armorType)
    {
        for (int i = 0; i < equippedArmor.Count; i++)
        {
            if (equippedArmor[i].ArmorType == armorType)
                return true;
        }
        return false;
    }

    public Tool GetEquippedTool(ToolType tooltype)
    {
        if (!HasToolEquipped(tooltype))
            return null;
        else
        {
            for (int i = 0; i < equippedTools.Count; i++)
            {
                if (equippedTools[i].ToolType == tooltype)
                    return equippedTools[i];
            }
            return null;
        }
    }

    public Armor GetEquippedArmorByType(ArmorType armorType)
    {
        if (!HasArmorEquipped(armorType))
            return null;
        else
        {
            for (int i = 0; i < equippedArmor.Count; i++)
            {
                if (equippedArmor[i].ArmorType == armorType)
                    return equippedArmor[i];
            }
            return null;
        }
    }

    public Weapon GetEquippedWeapon()
    {
        if (HasWeaponEquipped)
            return equippedWeapon;
        else return null;
    }

    public void UnEquipItem(ItemBase itemToUnequip, int index)
    {
        if(itemToUnequip.GetType() == typeof(Armor))
        {
            Armor equippedItem;
            if((equippedItem = equippedArmor.Find(x => x.Id == itemToUnequip.Id)) != null)
            {
                inventory.AddItemAtIndex(equippedItem.Id, index);
                equippedArmor.Remove(equippedItem);
                OnItemEquippedCallBack?.Invoke();
            }
        }

        else if (itemToUnequip.GetType() == typeof(Tool))
        {
            Tool equippedItem;
            if ((equippedItem = equippedTools.Find(x => x.Id == itemToUnequip.Id)) != null)
            {
                inventory.AddItemById(equippedItem.Id);
                equippedTools.Remove(equippedItem);
                OnItemEquippedCallBack?.Invoke();
            }
        }

        else if(itemToUnequip.GetType() == typeof(Weapon))
        {
            inventory.AddItemById(equippedWeapon.Id);
            equippedWeapon = null;
            OnItemEquippedCallBack?.Invoke();
        }
    }

    public void DropEquippedItem(ItemBase itemToUnequip)
    {
        if (itemToUnequip.GetType() == typeof(Armor))
        {
            Armor equippedItem;
            if ((equippedItem = equippedArmor.Find(x => x.Id == itemToUnequip.Id)) != null)
            {
                ItemFactory.CreateWorldObject(PlayerNetwork.PlayerObject.transform.position, equippedItem.Id);
                equippedArmor.Remove(equippedItem);
                OnItemEquippedCallBack?.Invoke();
            }
        }

        else if (itemToUnequip.GetType() == typeof(Tool))
        {
            Tool equippedItem;
            if ((equippedItem = equippedTools.Find(x => x.Id == itemToUnequip.Id)) != null)
            {
                ItemFactory.CreateWorldObject(PlayerNetwork.PlayerObject.transform.position, equippedItem.Id);
                equippedTools.Remove(equippedItem);
                OnItemEquippedCallBack?.Invoke();
            }
        }

        else if (itemToUnequip.GetType() == typeof(Weapon))
        {
            ItemFactory.CreateWorldObject(PlayerNetwork.PlayerObject.transform.position, equippedWeapon.Id);
            equippedWeapon = null;
            OnItemEquippedCallBack?.Invoke();
        }
    }
}

