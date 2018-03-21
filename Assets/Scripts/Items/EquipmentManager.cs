using System;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    private List<Tool> equippedTools;
    private Weapon equippedWeapon;
    private List<Armor> equippedArmor;

    private Inventory inventory;
    private EquipmentUI equipmentUI;

    public bool HasWeaponEquipped { get { return equippedWeapon != null; } }

    void Start()
    {
        equippedTools = new List<Tool>(2);
        equippedArmor = new List<Armor>(4);
    }

    public void EquipTool(Tool toolToEquip)
    {
        if (HasToolEquipped(toolToEquip.ToolType))
        {
            equippedTools.Remove(GetEquippedTool(toolToEquip.ToolType));
            inventory.AddItemById(toolToEquip.Id, 1);
            inventory.RemoveItemById(GetEquippedTool(toolToEquip.ToolType).Id);
            equippedTools.Add(toolToEquip);
        }
        else
        {
            equippedTools.Add(toolToEquip);
            inventory.RemoveItemById(toolToEquip.Id);
        }
    }

    public void EquipWeapon(Weapon weaponToEquip)
    {
        if (HasWeaponEquipped)
        {
            inventory.AddItemById(equippedWeapon.Id);
            equippedWeapon = null;
            equippedWeapon = weaponToEquip;
            inventory.RemoveItemById(weaponToEquip.Id);

        }
        else
        {
            inventory.RemoveItemById(weaponToEquip.Id);
            equippedWeapon = weaponToEquip;
        }

    }

    public void EquipArmor(Armor armorToEquip)
    {
        if (HasArmorEquipped(armorToEquip.ArmorType))
        {
            equippedArmor.Remove(GetEquippedArmor(armorToEquip.ArmorType));
            inventory.RemoveItemById(GetEquippedArmor(armorToEquip.ArmorType).Id);
            inventory.AddItemById(armorToEquip.Id, 1);
            equippedArmor.Add(armorToEquip);
        }
        else
        {
            equippedArmor.Add(armorToEquip);
            inventory.RemoveItemById(armorToEquip.Id);
        }
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

    public Armor GetEquippedArmor(ArmorType armorType)
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
}

