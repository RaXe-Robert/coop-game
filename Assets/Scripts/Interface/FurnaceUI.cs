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

    [SerializeField] private StatusBarProgress progressBar = null;

    public static FurnaceUI Instance { get; private set; }

    private Furnace furnace;
    public FuelInput FuelInput;
    public ItemInput ItemInput;
    public ItemOutput ItemOutput;

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
        if (furnace.FuelItem != null)
            FuelInput.CurrentItem = furnace.FuelItem;
        else
            FuelInput.Clear();
        if (furnace.InputItem != null)
            ItemInput.CurrentItem = furnace.InputItem;
        else
            ItemInput.Clear();
        if (furnace.OutputItem != null)
            ItemOutput.CurrentItem = furnace.OutputItem;
        else
            ItemOutput.Clear();
    }

    public void UpdateProgressBar()
    {
        progressBar.SetValue(furnace.MeltingProgress / 5F);
    }

    private void InitializeFurnace()
    {
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
        FuelInput = goFuelInputSlot.GetComponentInChildren<FuelInput>();

        goItemInputSlot = Instantiate(itemInputSlotPrefab, itemInputPrefab.transform);
        ItemInput = goItemInputSlot.GetComponentInChildren<ItemInput>();

        goItemOutputSlot = Instantiate(itemOutputSlotPrefab, itemOutputPrefab.transform);
        ItemOutput = goItemOutputSlot.GetComponentInChildren<ItemOutput>();

        FuelInput.Initialize(furnace);
        ItemInput.Initialize(furnace);
        ItemOutput.Initialize(furnace);

        UpdateUI();
        UpdateProgressBar();
    }

    public void OpenFurnace(Furnace f)
    {
        furnace = f;
        InitializeFurnace();
        furnace.OnItemChangedCallback += UpdateUI;
        furnace.OnMeltingCallback += UpdateProgressBar;
        GameInterfaceManager.Instance.ToggleGameInterface(GameInterface.Furnace);
    }

    public void CloseChest()
    {
        furnace.OnItemChangedCallback -= UpdateUI;
        furnace.OnMeltingCallback -= UpdateProgressBar;
        GameInterfaceManager.Instance.CloseAllInterfaces();
    }
}
