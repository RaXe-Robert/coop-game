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

    public void EquipArmor(Armor armorToEquip, int inventoryIndex)
    {
        if (HasArmorEquipped(armorToEquip.ArmorType))
        {
            var currentEquipped = GetEquippedArmorByType(armorToEquip.ArmorType);
            if (currentEquipped == armorToEquip)
                return;
            equippedArmor.Remove(currentEquipped);
            inventory.RemoveItemAtIndex(inventoryIndex);

            OnArmorUnequippedCallback?.Invoke(currentEquipped);
            inventory.AddItemById(currentEquipped.Id);
            equippedArmor.Add(armorToEquip);
        }
        else
        {
            equippedArmor.Add(armorToEquip);
            inventory.RemoveItemAtIndex(inventoryIndex);
        }

        SoundManager.Instance.PlaySound(SoundManager.Sound.EQUIP);
        OnItemChanged?.Invoke();
        OnArmorEquippedCallback?.Invoke(armorToEquip);
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

    public void UnequipArmor(Item itemToUnequip, int index)
    {
        Armor equippedItem;
        if((equippedItem = equippedArmor.Find(x => x.Id == itemToUnequip.Id)) != null)
        {
            inventory.AddItemAtIndex(equippedItem.Id, index);
            equippedArmor.Remove(equippedItem);
            OnArmorUnequippedCallback?.Invoke(equippedItem);
            OnItemChanged?.Invoke();
        }
    }

    public void DropEquippedItem(Item itemToUnequip)
    {
        if (itemToUnequip.GetType() == typeof(Armor))
        {
            Armor equippedItem;
            if ((equippedItem = equippedArmor.Find(x => x.Id == itemToUnequip.Id)) != null)
            {
                ItemFactory.CreateWorldObject(PlayerNetwork.LocalPlayer.transform.position, equippedItem.Id);
                equippedArmor.Remove(equippedItem);
                OnArmorUnequippedCallback?.Invoke(equippedItem);
                OnItemChanged?.Invoke();
            }
        }
    }
}

