using System;
using UnityEngine;

public class CraftingManager : MonoBehaviour
{
    public CraftingList availableRecipes;
    private CraftingQueue craftingQueue = new CraftingQueue();

    private void Start()
    {
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
        craftingQueue.AddRecipe(recipe);
    }
}
