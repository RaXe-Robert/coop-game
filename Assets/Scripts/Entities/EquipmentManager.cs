using System;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    #region callbacks
    //Equip
    public delegate void OnItemEquipped();
    public OnItemEquipped OnItemChanged;

    public delegate void OnWeaponEquipped(Weapon equippedWeapon);
    public OnWeaponEquipped OnWeaponEquippedCallback;

    public delegate void OnArmorEquipped(Armor equippedArmor);
    public OnArmorEquipped OnArmorEquippedCallback;

    public delegate void OnToolEquipped(Tool equippedTool);
    public OnToolEquipped OnToolEquippedCallback;

    //Unequip
    public delegate void OnItemUnequipped();
    public OnItemUnequipped OnItemUnequippedCallBack;

    public delegate void OnWeaponUnequipped(Weapon equippedWeapon);
    public OnWeaponUnequipped OnWeaponUnequippedCallback;

    public delegate void OnArmorUnEquipped(Armor equippedArmor);
    public OnArmorUnEquipped OnArmorUnequippedCallback;

    public delegate void OnToolUnEquipped(Tool equippedTool);
    public OnToolUnEquipped OnToolUnequippedCallback;

    #endregion callbacks

    private List<Tool> equippedTools;
    private Weapon equippedWeapon;
    private List<Armor> equippedArmor;

    private Inventory inventory;

    public bool HasWeaponEquipped { get { return equippedWeapon != null; } }

    private void Start()
    {
        equippedTools = new List<Tool>(2);
        equippedArmor = new List<Armor>(4);
        inventory = FindObjectOfType<Inventory>();
    }

    public void EquipTool(Tool toolToEquip, int inventoryIndex)
    {
        if (HasToolEquipped(toolToEquip.ToolType))
        {
            var currentEquipped = GetEquippedTool(toolToEquip.ToolType);
            if (currentEquipped == toolToEquip)
                return;

            inventory.RemoveEntityAtIndex(inventoryIndex);
            equippedTools.Remove(currentEquipped);

            inventory.AddEntityById(currentEquipped.Id, 1);
            equippedTools.Add(toolToEquip);
            OnToolUnequippedCallback?.Invoke(currentEquipped);
        }
        else
        {
            equippedTools.Add(toolToEquip);
            inventory.RemoveEntityAtIndex(inventoryIndex);
        }

        OnItemChanged?.Invoke();
        OnToolEquippedCallback?.Invoke(toolToEquip);
    }

    public void EquipWeapon(Weapon weaponToEquip, int inventoryIndex)
    {
        if (HasWeaponEquipped)
        {
            var currentEquipped = equippedWeapon;
            if (currentEquipped == weaponToEquip)
                return;
            inventory.RemoveEntityAtIndex(inventoryIndex);
            equippedWeapon = null;

            equippedWeapon = weaponToEquip;
            inventory.AddEntityById(currentEquipped.Id);
            OnWeaponUnequippedCallback?.Invoke(currentEquipped);
        }
        else
        {
            inventory.RemoveEntityAtIndex(inventoryIndex);
            equippedWeapon = weaponToEquip;
        }

        OnItemChanged?.Invoke();
        OnWeaponEquippedCallback?.Invoke(weaponToEquip);
    }

    public void EquipArmor(Armor armorToEquip, int inventoryIndex)
    {
        if (HasArmorEquipped(armorToEquip.ArmorType))
        {
            var currentEquipped = GetEquippedArmorByType(armorToEquip.ArmorType);
            if (currentEquipped == armorToEquip)
                return;
            equippedArmor.Remove(currentEquipped);
            inventory.RemoveEntityAtIndex(inventoryIndex);

            OnArmorUnequippedCallback?.Invoke(currentEquipped);
            inventory.AddEntityById(currentEquipped.Id);
            equippedArmor.Add(armorToEquip);
        }
        else
        {
            equippedArmor.Add(armorToEquip);
            inventory.RemoveEntityAtIndex(inventoryIndex);
        }

        OnItemChanged?.Invoke();
        OnArmorEquippedCallback?.Invoke(armorToEquip);
    }

    public void EquipItem(EntityBase entity, int inventoryIndex)
    {
        if (!entity.Equippable)
            return;

        if (entity.GetType() == typeof(Armor))
            EquipArmor(entity as Armor, inventoryIndex);
        else if (entity.GetType() == typeof(Weapon))
            EquipWeapon(entity as Weapon, inventoryIndex);
        else if (entity.GetType() == typeof(Tool))
            EquipTool(entity as Tool, inventoryIndex);
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

    public void UnEquipItem(EntityBase itemToUnequip, int index)
    {
        if(itemToUnequip.GetType() == typeof(Armor))
        {
            Armor equippedItem;
            if((equippedItem = equippedArmor.Find(x => x.Id == itemToUnequip.Id)) != null)
            {
                inventory.AddEntityAtIndex(equippedItem.Id, index);
                equippedArmor.Remove(equippedItem);
                OnArmorUnequippedCallback?.Invoke(equippedItem);
                OnItemChanged?.Invoke();
            }
        }

        else if (itemToUnequip.GetType() == typeof(Tool))
        {
            Tool equippedItem;
            if ((equippedItem = equippedTools.Find(x => x.Id == itemToUnequip.Id)) != null)
            {
                inventory.AddEntityAtIndex(equippedItem.Id, index);
                equippedTools.Remove(equippedItem);
                OnToolUnequippedCallback?.Invoke(equippedItem);
                OnItemChanged?.Invoke();
            }
        }

        else if(itemToUnequip.GetType() == typeof(Weapon))
        {
            inventory.AddEntityAtIndex(equippedWeapon.Id, index);
            OnWeaponUnequippedCallback?.Invoke(equippedWeapon);
            equippedWeapon = null;
            OnItemChanged?.Invoke();
        }
    }

    public void DropEquippedItem(EntityBase itemToUnequip)
    {
        if (itemToUnequip.GetType() == typeof(Armor))
        {
            Armor equippedItem;
            if ((equippedItem = equippedArmor.Find(x => x.Id == itemToUnequip.Id)) != null)
            {
                EntityFactory.CreateWorldObject(PlayerNetwork.PlayerObject.transform.position, equippedItem.Id);
                equippedArmor.Remove(equippedItem);
                OnArmorUnequippedCallback?.Invoke(equippedItem);
                OnItemChanged?.Invoke();
            }
        }

        else if (itemToUnequip.GetType() == typeof(Tool))
        {
            Tool equippedItem;
            if ((equippedItem = equippedTools.Find(x => x.Id == itemToUnequip.Id)) != null)
            {
                EntityFactory.CreateWorldObject(PlayerNetwork.PlayerObject.transform.position, equippedItem.Id);
                equippedTools.Remove(equippedItem);
                OnToolUnequippedCallback?.Invoke(equippedItem);
                OnItemChanged?.Invoke();
            }
        }

        else if (itemToUnequip.GetType() == typeof(Weapon))
        {
            EntityFactory.CreateWorldObject(PlayerNetwork.PlayerObject.transform.position, equippedWeapon.Id);
            OnWeaponUnequippedCallback?.Invoke(equippedWeapon);
            equippedWeapon = null;
            OnItemChanged?.Invoke();
        }
    }
}

