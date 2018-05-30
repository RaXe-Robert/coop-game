using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IFuelInput
{
    void AddFuel(Item item);
}

public class Furnace : BuildableWorldObject, IFuelInput {

    [SerializeField] private CraftingList availableRecipes;
    [SerializeField] private GameObject furnaceInterface;

    private float burningTime;

    protected override UnityAction[] InitializeActions()
    {
        return new UnityAction[]
        {
            new UnityAction(OpenInterface)
        };
    }

    private void Craft()
    {

    }

    private void OpenInterface()
    {

    }

    public void AddResource(Resource resouceToAdd)
    {

    }

    public void AddFuel(Item item)
    {
        throw new System.NotImplementedException();
    }
}
