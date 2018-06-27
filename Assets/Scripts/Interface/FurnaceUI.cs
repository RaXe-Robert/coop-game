using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FurnaceUI : MonoBehaviour {
    [SerializeField] private GameObject furnaceUI;

    [SerializeField] private GameObject fuelInputPrefab;
    [SerializeField] private GameObject itemInputPrefab;
    [SerializeField] private GameObject itemOutputPrefab;

    [SerializeField] private GameObject fuelInputSlotPrefab;
    [SerializeField] private GameObject itemInputSlotPrefab;
    [SerializeField] private GameObject itemOutputSlotPrefab;

    public static FurnaceUI Instance { get; private set; }

    private Furnace furnace;
    public FuelInput fuelInput;
    public ItemInput itemInput;
    public ItemOutput itemOutput;

    private GameObject goFuelInputSlot;
    private GameObject goItemInputSlot;
    private GameObject goItemOutputSlot;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    void Start () {
        GameInterfaceManager.Instance.AddInterface(furnaceUI, GameInterface.Furnace);

        if (furnace != null)
        {
            furnace.OnItemChangedCallback += UpdateUI;
        }
    }

    public void UpdateUI()
    {
        if (fuelInput != null)
            fuelInput.CurrentItem = furnace.FuelInput.CurrentItem;
        if (itemInput != null)
            itemInput.CurrentItem = furnace.ItemInput.CurrentItem;
        if (itemOutput != null)
            itemOutput.CurrentItem = furnace.ItemOutput.CurrentItem;
        else
        {
            fuelInput.Clear();
            itemInput.Clear();
            itemOutput.Clear();
        }
    }

    private void InitializeFurnace()
    {
        fuelInput = new FuelInput();
        itemInput = new ItemInput();
        itemOutput = new ItemOutput();

        if(goFuelInputSlot != null)
        {
            Destroy(goFuelInputSlot);
        }
        if (goItemInputSlot != null)
        {
            Destroy(goItemInputSlot);
        }
        if (goItemOutputSlot != null)
        {
            Destroy(goItemOutputSlot);
        }

        goFuelInputSlot = Instantiate(fuelInputSlotPrefab, fuelInputPrefab.transform);
        fuelInput = goFuelInputSlot.GetComponentInChildren<FuelInput>();

        goItemInputSlot = Instantiate(itemInputSlotPrefab, itemInputPrefab.transform);
        itemInput = goItemInputSlot.GetComponentInChildren<ItemInput>();

        goItemOutputSlot = Instantiate(itemOutputSlotPrefab, itemOutputPrefab.transform);
        itemOutput = goItemOutputSlot.GetComponentInChildren<ItemOutput>();

        fuelInput.Initialize(furnace);
        itemInput.Initialize(furnace);
        itemOutput.Initialize(furnace);

        UpdateUI();
    }

    public void OpenFurnace(Furnace f)
    {
        furnace = f;
        InitializeFurnace();
        furnace.OnItemChangedCallback += UpdateUI;
        GameInterfaceManager.Instance.ToggleGameInterface(GameInterface.Furnace);
    }

    public void CloseChest()
    {
        GameInterfaceManager.Instance.CloseAllInterfaces();
    }
}
