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
    private List<GameObject> goChestSlots;

    public void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    private void Start()
    {
        GameInterfaceManager.Instance.AddInterface(chestUI, GameInterface.Chest);
        equipmentManager = FindObjectOfType<EquipmentManager>();

        if(chest != null)
        {
            chest.OnItemChangedCallback += UpdateUI;
        }
    }

    public void UpdateUI()
    {
        for (int i = 0; i < Chest.ChestSize; i++)
        {
            if (i < chest.chestItems.Count)
                chestSlots[i].CurrentItem = chest.chestItems[i];
            else chestSlots[i].Clear();
        }
    }

    private void InitializeChest()
    {
        chestSlots = new List<ChestItemSlot>(Chest.ChestSize);
        if (goChestSlots != null)
        {
            foreach (GameObject go in goChestSlots)
            {
                Destroy(go);
            }
        }
        goChestSlots = new List<GameObject>();

        for (int i = 0; i < Chest.ChestSize; i++)
        {
            var go = Instantiate(chestSlotPrefab, chestUI.transform);
            goChestSlots.Add(go);
            chestSlots.Add(go.GetComponentInChildren<ChestItemSlot>());
            chestSlots[i].Initialize(i, chest, equipmentManager);
        }

        UpdateUI();
    }

    public void OpenChest(Chest c)
    {
        chest = c;
        InitializeChest();
        chest.OnItemChangedCallback += UpdateUI;
        GameInterfaceManager.Instance.ToggleGameInterface(GameInterface.Chest);
    }

    public void CloseChest()
    {
        chest.OnItemChangedCallback -= UpdateUI;
        GameInterfaceManager.Instance.CloseAllInterfaces();
    }
}
