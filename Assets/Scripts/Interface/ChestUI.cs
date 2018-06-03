using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestUI : MonoBehaviour {
    [SerializeField] private GameObject chestSlotPrefab;
    [SerializeField] private GameObject chestUI;

    public static ChestUI Instance { get; private set; }

    private Chest chest;
    private EquipmentManager equipmentManager;
    private List<ChestItemSlot> chestSlots;

    public void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    private void Start()
    {
        equipmentManager = FindObjectOfType<EquipmentManager>();
        chestSlots = new List<ChestItemSlot>(Chest.ChestSize);     
    }

    public void UpdateUI()
    {
        for (int i = Chest.ChestSize; i < Chest.ChestSize; i++)
        {
            if (i < chest.chestItems.Count)
                chestSlots[i].CurrentItem = chest.chestItems[i];
            else chestSlots[i].Clear();
        }
    }

    private void InitializeChest()
    {
        for (int i = 0; i < Chest.ChestSize; i++)
        {
            var go = Instantiate(chestSlotPrefab, chestUI.transform);
            chestSlots.Add(go.GetComponentInChildren<ChestItemSlot>());
            chestSlots[i].Initialize(i, chest, equipmentManager);
        }
    }

    private void DeInitializeChest()
    {
        
    }

    public void OpenChest(Chest c)
    {
        chest = c;
        InitializeChest();
        chest.OnItemChangedCallback += UpdateUI;
        chestUI.SetActive(true);
    }

    public void CloseChest()
    {
        DeInitializeChest();
        chestUI.SetActive(false);
    }
}
