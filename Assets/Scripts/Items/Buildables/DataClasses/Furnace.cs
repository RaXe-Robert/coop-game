﻿using System.Collections;
using System.Collections.Generic;
using Photon;
using UnityEngine;
using UnityEngine.Events;

public class Furnace : BuildableWorldObject {
    [SerializeField] private CraftingList availableRecipes;
    [SerializeField] private GameObject fireParticlesPrefab;
    private GameObject activeFireParticles = null;

    public float MeltingProgress;
    public float BurningTime { get; set; }
    public bool IsMelting { get; private set; } = false;

    public Item FuelItem;
    public Item InputItem;
    public Item OutputItem;

    public Item CurrentItem { get; set; }

    public delegate void OnItemChanged();
    public OnItemChanged OnItemChangedCallback;

    public delegate void OnMelting();
    public OnMelting OnMeltingCallback;

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
        CloseFurnace();
        DestroyWorldObject();
        DropAllItems();
    }

    private void OpenFurnace()
    {
        FurnaceUI.Instance.OpenFurnace(this);
    }

    private void CloseFurnace()
    {
        FurnaceUI.Instance.CloseFurnace();
    }

    private void Update()
    {    
        HandleItems();
        HandleFuel();
        HandleMeltingProgress();
        ToggleFire();
    }

    private void ToggleFire()
    {
        if (!IsMelting && activeFireParticles != null)
            Destroy(activeFireParticles);

        else if (IsMelting && activeFireParticles == null)
        {
            if (fireParticlesPrefab)
            {
                activeFireParticles = Instantiate(fireParticlesPrefab, transform);
            }
        }
    }

    private void HandleMeltingProgress()
    {
        if (BurningTime <= 0 || CurrentItem == null || InputItem == null)
        {
            IsMelting = false;
            return;
        }

        MeltingProgress += BurningTime > 0 ? Time.deltaTime : -Time.deltaTime;
        IsMelting = true;
        if (MeltingProgress >= 5)
        {
            MeltingProgress = 0;
            DepositItem(ItemFactory.CreateNewItem(CurrentItem.MeltingResult.Id, 1));            
            CurrentItem = null;
        }
        
        OnMeltingCallback?.Invoke();
    }

    private void HandleItems()
    {
        if (CurrentItem == null && BurningTime > 0)
        {
            if (InputItem?.MeltingResult != null)
            {
                CurrentItem = TakeItem();
            }
        }
    }

    private void HandleFuel()
    {
        //If we have no remaining fuel but there is some left in the furnace and there is a meltable item we can start consuming fuel.
        if (BurningTime <= 0 && FuelItem != null && InputItem?.MeltingResult != null)
        {
            BurningTime += FuelItem.BurningTime;
            TakeFuel();
        }
        else if (BurningTime > 0)
            BurningTime -= Time.deltaTime;
    }

    private void OpenInterface()
    {
        GameInterfaceManager.Instance.ToggleGameInterface(GameInterface.Furnace);
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
