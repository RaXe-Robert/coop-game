using System.Collections;
using System.Collections.Generic;
using Photon;
using UnityEngine;
using UnityEngine.Events;

public class Furnace : BuildableWorldObject, IFuelInput, IItemInput {
    [SerializeField] private CraftingList availableRecipes;
    [SerializeField] private GameObject furnaceInterfaceprefab;

    private Canvas canvas;
    private GameObject furnaceInterface;
    private float meltingProgress;

    public FuelInput FuelInput { get; set; }
    public float BurningTime { get; set; }
    public ItemInput ItemInput { get; set; }
    public Item CurrentItem { get; set; }
    public ItemOutput ItemOutput { get; set; }

    protected override void Start()
    {
        base.Start();
        canvas = FindObjectOfType<Canvas>();

        furnaceInterface = Instantiate(furnaceInterfaceprefab, canvas.transform);
        furnaceInterface.SetActive(false);

        FuelInput = furnaceInterface.GetComponentInChildren<FuelInput>();
        ItemInput = furnaceInterface.GetComponentInChildren<ItemInput>();
        ItemOutput = furnaceInterface.GetComponentInChildren<ItemOutput>();
    }

    protected override UnityAction[] InitializeActions()
    {
        return new UnityAction[]
        {
            new UnityAction(OpenInterface)
        };
    }

    private void Update()
    {
        HandleItems();
        HandleFuel();
        HandleMeltingProgress();
    }

    private void HandleMeltingProgress()
    {
        if(CurrentItem != null)
        {
            meltingProgress += BurningTime > 0 ? Time.deltaTime : -Time.deltaTime;
            if(meltingProgress >= 5)
            {
                ItemOutput.DepositItem(ItemFactory.CreateNewItem(CurrentItem.MeltingResult.Id, 1));
                CurrentItem = null;
                meltingProgress = 0;
                Debug.Log("FinishedMelting");
            }
        }
    }

    private void HandleItems()
    {
        if(CurrentItem == null)
        {
            if(ItemInput?.CurrentItem?.Name == "Iron Ore")
                CurrentItem = ItemInput.TakeItem();
        }
    }

    private void HandleFuel()
    {
        if (BurningTime <= 0 && FuelInput.CurrentItem != null && CurrentItem != null)
        {
            FuelInput.TakeFuel();
            BurningTime += FuelInput.CurrentItem.BurningTime;
        }
        else if (BurningTime > 0)
        {
            BurningTime -= Time.deltaTime;
            Debug.Log(BurningTime);
        }
    }

    private void OpenInterface()
    {
        GameInterfaceManager.Instance.AddInterface(furnaceInterface, GameInterface.Furnace);
        GameInterfaceManager.Instance.ToggleGameInterface(GameInterface.Furnace);
    }

    private void MeltItem()
    {
        if(ItemInput.CurrentItem.Name == "Iron ore")
        {
            ItemInput.TakeItem();
        }
    }
}
