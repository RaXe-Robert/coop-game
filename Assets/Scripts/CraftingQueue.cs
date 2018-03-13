using System;
using System.Collections.Generic;
using UnityEngine;

public class CraftingQueue
{
    public CraftingQueue(Inventory inventory)
    {
        this.inventory = inventory;
    }

    private Queue<CraftingRecipe> craftQueue = new Queue<CraftingRecipe>();
    private CraftingRecipe currentCraft;
    private float craftingProgress;
    private Inventory inventory;

    public void AddRecipe(CraftingRecipe recipe)
    {
        Debug.Log($"Added {recipe.resultItem.item.name} x {recipe.amountToCraft} to the crafting queue");
        craftQueue.Enqueue(recipe);
    }

    public void Update()
    {
        //Nothing to craft
        if (craftQueue.Count == 0 && currentCraft == null)
            return;

        //We aren't crafting anything, start a new craft.
        if(currentCraft == null)
        {
            currentCraft = craftQueue.Dequeue();
            Debug.Log($"Current craft is now {currentCraft.resultItem.item.name}");
        }
        else
        {
            if(currentCraft.amountToCraft > 0 && craftingProgress <= 0)
            {
                craftingProgress = currentCraft.craftingTime;
                Debug.Log($"Starting new craft: {currentCraft.amountToCraft} {currentCraft.resultItem.item.name} remaining");
            }
            else if(currentCraft.amountToCraft == 0 && craftingProgress <= 0)
            {
                Debug.Log("Done crafting");
                currentCraft = null;
            }
            else
            {
                craftingProgress -= Time.deltaTime;
                if (craftingProgress <= 0)
                {
                    inventory.AddItemById(currentCraft.resultItem.item.Id, currentCraft.resultItem.amount);
                    currentCraft.amountToCraft--;
                }
            }
        }
    }
}

