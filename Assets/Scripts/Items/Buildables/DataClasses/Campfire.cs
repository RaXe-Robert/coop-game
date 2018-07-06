using System.Collections;
using System.Collections.Generic;
using Photon;
using UnityEngine;
using UnityEngine.Events;

public class Campfire : BuildableWorldObject
{
    [SerializeField] private CraftingList availableRecipes;
    [SerializeField] private GameObject fireParticlesPrefab;
    private GameObject activeFireParticles = null;

    public float CookingProgress;
    public float BurningTime { get; set; }
    public bool IsCooking { get; private set; } = false;

    public Item FuelItem;
    public Item InputItem;
    public Item OutputItem;

    public Item CurrentItem { get; set; }

    public delegate void OnItemChanged();
    public OnItemChanged OnItemChangedCallback;

    public delegate void OnCooking();
    public OnCooking OnCookingCallback;

    protected override UnityAction[] InitializeActions()
    {
        return new UnityAction[]
        {
            new UnityAction(OpenCampfire),
            new UnityAction(CloseCampfire)
        };
    }

    protected override void Pickup()
    {
        CloseCampfire();
        DestroyWorldObject();
        DropAllItems();
    }

    private void OpenCampfire()
    {
        CampfireUI.Instance.OpenCampfire(this);
    }

    private void CloseCampfire()
    {
        CampfireUI.Instance.CloseCampfire();
    }

    private void Update()
    {
        HandleItems();
        HandleFuel();
        HandleCookingProgress();
        ToggleFire();
    }

    private void ToggleFire()
    {
        if (!IsCooking && activeFireParticles != null)
            Destroy(activeFireParticles);

        else if (IsCooking && activeFireParticles == null)
        {
            if (fireParticlesPrefab)
            {
                activeFireParticles = Instantiate(fireParticlesPrefab, transform);
            }
        }
    }

    private void HandleCookingProgress()
    {
        if (BurningTime <= 0 || CurrentItem == null || InputItem == null)
        {
            IsCooking = false;
            return;
        }

        CookingProgress += BurningTime > 0 ? Time.deltaTime : -Time.deltaTime;
        IsCooking = true;
        if (CookingProgress >= 5)
        {
            CookingProgress = 0;
            DepositItem(ItemFactory.CreateNewItem(CurrentItem.CookingResult.Id, 1));
            CurrentItem = null;
        }

        OnCookingCallback?.Invoke();
    }

    private void HandleItems()
    {
        if (CurrentItem == null && BurningTime > 0)
        {
            if (InputItem?.CookingResult != null)
            {
                CurrentItem = TakeItem();
            }
        }
    }

    private void HandleFuel()
    {
        //If we have no remaining fuel but there is some left in the furnace and there is a meltable item we can start consuming fuel.
        if (BurningTime <= 0 && FuelItem != null && InputItem?.CookingResult != null)
        {
            BurningTime += FuelItem.BurningTime;
            TakeFuel();
        }
        else if (BurningTime > 0)
            BurningTime -= Time.deltaTime;
    }

    private void OpenInterface()
    {
        GameInterfaceManager.Instance.ToggleGameInterface(GameInterface.Campfire);
    }

    public Item TakeItem()
    {
        if (InputItem == null)
            return null;
        else
        {
            if (InputItem.StackSize > 0)
                InputItem.StackSize--;
            else
                InputItem = null;

            OnItemChangedCallback?.Invoke();
            return InputItem != null ? ItemFactory.CreateNewItem(InputItem.Id) : null;
        }
    }

    public void TakeFuel()
    {
        if (FuelItem == null)
            return;
        else
        {
            if (FuelItem.StackSize > 1)
                FuelItem.StackSize--;
            else
                FuelItem = null;

            OnItemChangedCallback?.Invoke();
        }
    }

    public void DepositItem(Item itemToDeposit)
    {
        if (OutputItem != null)
        {
            if (itemToDeposit.Id != OutputItem?.Id)
                return;
            OutputItem.StackSize += itemToDeposit.StackSize;
        }
        else OutputItem = itemToDeposit;

        OnItemChangedCallback?.Invoke();
    }

    public void DropAllItems()
    {
        if (FuelItem != null)
        {
            ItemFactory.CreateWorldObject(transform.position + new Vector3(Random.Range(-2, 2), 0, Random.Range(-2, 2)), FuelItem.Id, FuelItem.StackSize);
            FuelItem = null;
        }
        if (InputItem != null)
        {
            ItemFactory.CreateWorldObject(transform.position + new Vector3(Random.Range(-2, 2), 0, Random.Range(-2, 2)), InputItem.Id, InputItem.StackSize);
            InputItem = null;
        }
        if (OutputItem != null)
        {
            ItemFactory.CreateWorldObject(transform.position + new Vector3(Random.Range(-2, 2), 0, Random.Range(-2, 2)), OutputItem.Id, OutputItem.StackSize);
            OutputItem = null;
        }

        OnItemChangedCallback?.Invoke();
    }
}
