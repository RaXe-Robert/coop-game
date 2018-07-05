using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampfireUI : MonoBehaviour
{
    [SerializeField] private GameObject campfireUI;

    [SerializeField] private GameObject fuelInputPrefab;
    [SerializeField] private GameObject itemInputPrefab;
    [SerializeField] private GameObject itemOutputPrefab;

    [SerializeField] private GameObject fuelInputSlotPrefab;
    [SerializeField] private GameObject itemInputSlotPrefab;
    [SerializeField] private GameObject itemOutputSlotPrefab;

    [SerializeField] private StatusBarProgress progressBar = null;

    public static CampfireUI Instance { get; private set; }

    private Campfire campfire;
    public CampfireFuelInput FuelInput;
    public CampfireItemInput ItemInput;
    public CampfireItemOutput ItemOutput;

    private GameObject goFuelInputSlot;
    private GameObject goItemInputSlot;
    private GameObject goItemOutputSlot;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    void Start()
    {
        GameInterfaceManager.Instance.AddInterface(campfireUI, GameInterface.Campfire);

        if (campfire != null)
        {
            campfire.OnItemChangedCallback += UpdateUI;
        }
    }

    public void UpdateUI()
    {
        if (campfire.FuelItem != null)
            FuelInput.CurrentItem = campfire.FuelItem;
        else
            FuelInput.Clear();
        if (campfire.InputItem != null)
            ItemInput.CurrentItem = campfire.InputItem;
        else
            ItemInput.Clear();
        if (campfire.OutputItem != null)
            ItemOutput.CurrentItem = campfire.OutputItem;
        else
            ItemOutput.Clear();
    }

    public void UpdateProgressBar()
    {
        progressBar.SetValue(campfire.CookingProgress / 5F);
    }

    private void InitializeCampfire()
    {
        if (goFuelInputSlot != null)
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
        FuelInput = goFuelInputSlot.GetComponentInChildren<CampfireFuelInput>();

        goItemInputSlot = Instantiate(itemInputSlotPrefab, itemInputPrefab.transform);
        ItemInput = goItemInputSlot.GetComponentInChildren<CampfireItemInput>();

        goItemOutputSlot = Instantiate(itemOutputSlotPrefab, itemOutputPrefab.transform);
        ItemOutput = goItemOutputSlot.GetComponentInChildren<CampfireItemOutput>();

        FuelInput.Initialize(campfire);
        ItemInput.Initialize(campfire);
        ItemOutput.Initialize(campfire);

        UpdateUI();
        UpdateProgressBar();
    }

    public void OpenCampfire(Campfire c)
    {
        campfire = c;
        InitializeCampfire();
        campfire.OnItemChangedCallback += UpdateUI;
        campfire.OnCookingCallback += UpdateProgressBar;
        GameInterfaceManager.Instance.ToggleGameInterface(GameInterface.Campfire);
    }

    public void CloseCampfire()
    {
        campfire.OnItemChangedCallback -= UpdateUI;
        campfire.OnCookingCallback -= UpdateProgressBar;
        GameInterfaceManager.Instance.CloseAllInterfaces();
    }
}
