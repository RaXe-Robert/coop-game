using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentUI : MonoBehaviour {

    [SerializeField] private ArmorItemSlot[] armorSlots;
    [SerializeField] private WeaponItemSlot weaponSlot;
    [SerializeField] private ToolItemSlot[] toolSlots;

    private EquipmentManager equipmentManager;
    private Inventory inventory;

    private void Start () {
        equipmentManager = FindObjectOfType<EquipmentManager>();

        inventory = FindObjectOfType<Inventory>();
        equipmentManager.OnItemChanged += UpdateUI;

        //Initialize all the equipment slots so they have a reference to the equipmentManager
        for (int i = 0; i < armorSlots.Length; i++)
        {
            armorSlots[i].Initialize(-1, inventory, equipmentManager);
        }
        for (int i = 0; i < toolSlots.Length; i++)
        {
            toolSlots[i].Initialize(-1, inventory, equipmentManager);
        }
        weaponSlot.Initialize(-1, inventory, equipmentManager);
    }

    private void UpdateUI()
    {
        UpdateTools();
        UpdateWeapon();
        UpdateArmor();
    }

    private void UpdateTools()
    {
        for (int i = 0; i < toolSlots.Length; i++)
        {
            if (equipmentManager.HasToolEquipped(toolSlots[i].SlotType))
            {
                toolSlots[i].CurrentItem = equipmentManager.GetEquippedTool(toolSlots[i].SlotType);
            }
            else toolSlots[i].Clear();
        }
    }

    private void UpdateWeapon()
    {
        if (equipmentManager.HasWeaponEquipped)
            weaponSlot.CurrentItem = equipmentManager.GetEquippedWeapon();
        else weaponSlot.Clear();
    }

    private void UpdateArmor()
    {
        for (int i = 0; i < armorSlots.Length; i++)
        {
            if (equipmentManager.HasArmorEquipped(armorSlots[i].SlotType))
            {
                armorSlots[i].CurrentItem = equipmentManager.GetEquippedArmorByType(armorSlots[i].SlotType);
            }
            else armorSlots[i].Clear();
        }
    }
}
