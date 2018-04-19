using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// UI for displaying the recipes that are in the crafting queue
/// </summary>
public class CraftingUI : MonoBehaviour, IPointerEnterHandler
{
    [Header("Prefabs")]
    [SerializeField] private GameObject craftingQueueEntityPrefab;
    [SerializeField] private GameObject recipeSlotPrefab;

    [Header("Parents")]
    [SerializeField] private GameObject recipeSlotParent;
    [SerializeField] private GameObject craftingQueue;

    private CraftingManager craftingManager;
    private Inventory inventory;

    private List<CraftingTooltip> currentCrafts;

    private void Start()
    {
        inventory = FindObjectOfType<Inventory>();
        craftingManager = FindObjectOfType<CraftingManager>();

        currentCrafts = new List<CraftingTooltip>();

        craftingManager.OnCraftAddedCallback += AddToVisualCraftingQueue;
        craftingManager.OnCraftCompletedCallback += RemoveFromVisualCraftingQueue;

        for (int i = 0; i < craftingManager.AvailableRecipes.recipes.Count; i++)
        {
            CraftingRecipeSlot recipeSlot = Instantiate(recipeSlotPrefab, recipeSlotParent.transform).GetComponent<CraftingRecipeSlot>();
            recipeSlot.Initialize(craftingManager.AvailableRecipes.recipes[i], inventory);
        }
    }

    private void AddToVisualCraftingQueue(CraftingRecipe recipe)
    {
        CraftingTooltip craftingTooltip = Instantiate(craftingQueueEntityPrefab, craftingQueue.transform).GetComponent<CraftingTooltip>();
        craftingTooltip.Initialize(recipe);
        currentCrafts.Add(craftingTooltip);
    }

    private void RemoveFromVisualCraftingQueue(CraftingRecipe recipe)
    {
        // Dont remove if there is still more of the same entity in the queue
        if (craftingManager.CraftingQueue.AmountRemaining > 0)
            return;

        Destroy(currentCrafts[0].gameObject);
        currentCrafts.RemoveAt(0);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Tooltip.Instance.Hide();
    }
}

