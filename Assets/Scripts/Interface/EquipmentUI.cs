using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentUI : MonoBehaviour {

    [SerializeField] private ArmorItemSlot[] armorSlots;

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
    }

    private void UpdateUI()
    {
        UpdateArmor();
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
