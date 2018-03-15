﻿using System;
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
        craftingQueue = new CraftingQueue();

        craftingQueue.OnCraftCompletedCallback += CompleteCraft;
    }

    private void CompleteCraft(CraftingRecipe recipe)
    {
        inventory.AddItemById(recipe.result.item.Id, recipe.result.amount);
    }

    private void Update()
    {
        craftingQueue.UpdateQueue();
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
