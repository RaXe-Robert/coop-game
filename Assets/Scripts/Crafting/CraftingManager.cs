using System;
using UnityEngine;

[RequireComponent(typeof(Inventory))]
public class CraftingManager : MonoBehaviour
{
    public CraftingList AvailableRecipes;
    public CraftingQueue CraftingQueue { get; private set; }
    private Inventory inventory;

    public delegate void OnCraftCompleted(CraftingRecipe recipe);
    public OnCraftCompleted OnCraftCompletedCallback;

    public delegate void OnCraftAdded(CraftingRecipe recipe);
    public OnCraftAdded OnCraftAddedCallback;

    private void Start()
    {
        inventory = GetComponent<Inventory>();
        CraftingQueue = new CraftingQueue(this);

        OnCraftCompletedCallback += CompleteCraft;
    }

    private void CompleteCraft(CraftingRecipe recipe)
    {
        inventory.AddItemById(recipe.result.item.Id, recipe.result.amount);
    }

    private void Update()
    {
        CraftingQueue.UpdateQueue();
    }

    /// <summary>
    /// Add a craftingRecipe to the crafting queue
    /// </summary>
    /// <param name="recipe"></param>
    public void AddToQueue(CraftingRecipe recipe)
    {
        if (inventory.RemoveEntitiesForCrafting(recipe))
            CraftingQueue.AddRecipe(recipe);
    }
}
