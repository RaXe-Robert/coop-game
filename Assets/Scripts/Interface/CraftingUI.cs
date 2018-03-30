using System;
using System.Collections.Generic;
using UnityEngine;

public class CraftingUI : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject craftingQueueItemPrefab;
    [SerializeField] private GameObject recipeSlotPrefab;

    [Header("Parents")]
    [SerializeField] private GameObject recipeSlotParent;
    [SerializeField] private GameObject craftingQueue;

    private CraftingManager craftingManager;
    private Inventory inventory;

    private List<GameObject> currentCrafts;

    private void Start()
    {
        inventory = FindObjectOfType<Inventory>();
        craftingManager = FindObjectOfType<CraftingManager>();

        currentCrafts = new List<GameObject>();

        craftingManager.OnCraftAddedCallback += AddToVisualCraftingQueue;
        craftingManager.OnCraftCompletedCallback += RemoveFromVisualCraftingQueue;

        for (int i = 0; i < craftingManager.availableRecipes.recipes.Count; i++)
        {
            CraftingRecipeSlot recipeSlot = Instantiate(recipeSlotPrefab, recipeSlotParent.transform).GetComponent<CraftingRecipeSlot>();
            recipeSlot.Initialize(craftingManager.availableRecipes.recipes[i], inventory);
        }
    }

    private void AddToVisualCraftingQueue(CraftingRecipe recipe)
    {
        CraftingTooltip craftingTooltip = Instantiate(craftingQueueItemPrefab, craftingQueue.transform).GetComponent<CraftingTooltip>();
        craftingTooltip.Initialize(recipe);
        currentCrafts.Add(craftingTooltip.gameObject);
    }

    private void RemoveFromVisualCraftingQueue(CraftingRecipe recipe)
    {
        Destroy(currentCrafts[0].gameObject);
        currentCrafts.RemoveAt(0);
    }
}

