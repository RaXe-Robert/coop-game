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

        //TODO: Maybe find a better way than this.
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
        if (BurningTime <= 0 || CurrentItem == null)
            return;

        meltingProgress += BurningTime > 0 ? Time.deltaTime : -Time.deltaTime;
        if(meltingProgress >= 5)
        {
            ItemOutput.DepositItem(ItemFactory.CreateNewItem(CurrentItem.MeltingResult.Id, 1));
            meltingProgress = 0;
            CurrentItem = null;
        }
    }

    private void HandleItems()
    {
        if(CurrentItem == null && BurningTime > 0)
            if(ItemInput?.CurrentItem?.MeltingResult != null)
                CurrentItem = ItemInput.TakeItem();
    }

    private void HandleFuel()
    {
        //If we have no remaining fuel but there is some left in the furnace and there is a meltable item we can start consuming fuel.
        if (BurningTime <= 0 && FuelInput.CurrentItem != null && ItemInput?.CurrentItem?.MeltingResult != null)
        {
            BurningTime += FuelInput.CurrentItem.BurningTime;
            FuelInput.TakeFuel();
        }
        else if(BurningTime > 0)
            BurningTime -= Time.deltaTime;
    }

    private void OpenInterface()
    {
        GameInterfaceManager.Instance.AddInterface(furnaceInterface, GameInterface.Furnace);
        GameInterfaceManager.Instance.ToggleGameInterface(GameInterface.Furnace);
    }

    protected override void Pickup()
    {
        // If null the action will be cancelled
        if (BuildableInteractionMenu.Instance?.Target == null)
            return;

        BuildableInteractionMenu.Instance.Target.DestroyWorldObject();
        GameInterfaceManager.Instance.CloseAllInterfaces();
    }
}
