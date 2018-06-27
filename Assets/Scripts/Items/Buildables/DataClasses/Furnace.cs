using System.Collections;
using System.Collections.Generic;
using Photon;
using UnityEngine;
using UnityEngine.Events;

public class Furnace : BuildableWorldObject {
    [SerializeField] private CraftingList availableRecipes;
    
    private float meltingProgress;

    public float BurningTime { get; set; }

    public FuelInput FuelInput { get; set; }
    public ItemInput ItemInput { get; set; }
    public ItemOutput ItemOutput { get; set; }
    public Item CurrentItem { get; set; }

    public delegate void OnItemChanged();
    public OnItemChanged OnItemChangedCallback;

    protected override void Start()
    {
        base.Start();
        SetFields();
    }

    public void SetFields()
    {
        FuelInput = new FuelInput();
        ItemInput = new ItemInput();
        ItemOutput = new ItemOutput();
    }

    protected override UnityAction[] InitializeActions()
    {
        return new UnityAction[]
        {
            new UnityAction(OpenFurnace),
            new UnityAction(CloseFurnace)
        };
    }

    protected override void Pickup()
    {
        // If null the action will be cancelled
        if (BuildableInteractionMenu.Instance?.Target == null)
            return;

        CloseFurnace();
        BuildableInteractionMenu.Instance.Target.DestroyWorldObject();

        //Should drop the stuff in the furnace when it gets picked up.
        //DropAllItems();
    }

    private void OpenFurnace()
    {
        FurnaceUI.Instance.OpenFurnace(this);
    }

    private void CloseFurnace()
    {
        FurnaceUI.Instance.CloseChest();
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
        if (meltingProgress >= 5)
        {
            ItemOutput.DepositItem(ItemFactory.CreateNewItem(CurrentItem.MeltingResult.Id, 1));
            meltingProgress = 0;
            CurrentItem = null;
            OnItemChangedCallback?.Invoke();
        }
    }

    private void HandleItems()
    {
        if (CurrentItem == null && BurningTime > 0)
            if (ItemInput?.CurrentItem?.MeltingResult != null)
            {
                CurrentItem = ItemInput.TakeItem();
                OnItemChangedCallback?.Invoke();
            }
    }

    private void HandleFuel()
    {
        //If we have no remaining fuel but there is some left in the furnace and there is a meltable item we can start consuming fuel.
        if (BurningTime <= 0 && FuelInput.CurrentItem != null && ItemInput?.CurrentItem?.MeltingResult != null)
        {
            BurningTime += FuelInput.CurrentItem.BurningTime;
            FuelInput.TakeFuel();
            OnItemChangedCallback?.Invoke();
        }
        else if (BurningTime > 0)
            BurningTime -= Time.deltaTime;
    }

    private void OpenInterface()
    {
        GameInterfaceManager.Instance.ToggleGameInterface(GameInterface.Furnace);
    }
}
