using System;
using UnityEngine;

[RequireComponent(typeof(Inventory))]
public class CraftingManager : MonoBehaviour
{
    public CraftingList availableRecipes;
    private CraftingQueue craftingQueue;
    private Inventory inventory;

    private void Start()
    {
        inventory = GetComponent<Inventory>();
        craftingQueue = new CraftingQueue(inventory);
    }

    private void Update()
    {
        craftingQueue.Update();
    }

    /// <summary>
    /// Add a craftingRecipe to the crafting queue
    /// </summary>
    /// <param name="recipe"></param>
    public void AddToQueue(CraftingRecipe recipe)
    {
        if (inventory.RemoveItemsForCrafting(recipe))
            craftingQueue.AddRecipe(recipe);
    }
}
