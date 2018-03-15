using System;
using System.Collections.Generic;
using UnityEngine;

public class CraftingQueue
{
    private Queue<CraftingRecipe> craftQueue = new Queue<CraftingRecipe>();
    private CraftingRecipe currentCraft;
    private float craftingProgress;

    private int amountToCraft = 1;

    public delegate void OnCraftCompleted(CraftingRecipe recipe);
    public OnCraftCompleted OnCraftCompletedCallback;

    public void AddRecipe(CraftingRecipe recipe)
    {
        Debug.Log($"Added {recipe.result.item.name} x {recipe.amountToCraft} to the crafting queue");
        craftQueue.Enqueue(recipe);
    }

    public void UpdateQueue()
    {
        //Nothing to craft
        if (craftQueue.Count == 0 && currentCraft == null)
            return;

        //We aren't crafting anything, start a new craft.
        if(currentCraft == null)
        {
            currentCraft = craftQueue.Dequeue();
            amountToCraft = currentCraft.amountToCraft;
            craftingProgress = currentCraft.craftingTime;
        }
        else
        {
            craftingProgress -= Time.deltaTime;
            //Done with a craft.
            if(craftingProgress <= 0)
            {
                amountToCraft--;
                OnCraftCompletedCallback?.Invoke(currentCraft);

                if (amountToCraft > 0)
                    craftingProgress = currentCraft.craftingTime;
                else
                    currentCraft = null;
            }
        }
    }
}

