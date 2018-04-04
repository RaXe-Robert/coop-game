﻿using System;
using System.Collections.Generic;
using UnityEngine;

public class CraftingQueue
{
    public CraftingQueue(CraftingManager craftingManager)
    {
        this.craftingManager = craftingManager;
    }

    private Queue<CraftingRecipe> craftQueue = new Queue<CraftingRecipe>();
    private CraftingRecipe currentCraft;
    private float craftingProgress;
    private CraftingManager craftingManager;

    public int AmountRemaining { get; private set; } = 1;
    
    public void AddRecipe(CraftingRecipe recipe)
    {
        craftingManager.OnCraftAddedCallback?.Invoke(recipe);
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
            AmountRemaining = currentCraft.amountToCraft;
            craftingProgress = currentCraft.craftingTime;
        }
        else
        {
            craftingProgress -= Time.deltaTime;
            //Done with a craft.
            if(craftingProgress <= 0)
            {
                AmountRemaining--;
                craftingManager.OnCraftCompletedCallback?.Invoke(currentCraft);

                if (AmountRemaining > 0)
                    craftingProgress = currentCraft.craftingTime;
                else
                    currentCraft = null;
            }
        }
    }
}

