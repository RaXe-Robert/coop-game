using System;
using System.Collections.Generic;
using UnityEngine;

public class CraftingUI : MonoBehaviour
{
    [SerializeField] private GameObject recipeSlotParent;
    [SerializeField] private GameObject recipeSlotPrefab;

    private CraftingManager craftingManager;
    private Inventory inventory;

    private void Start()
    {
        inventory = FindObjectOfType<Inventory>();
        craftingManager = FindObjectOfType<CraftingManager>();

        for (int i = 0; i < craftingManager.availableRecipes.recipes.Count; i++)
        {
            var slot = Instantiate(recipeSlotPrefab, recipeSlotParent.transform);
            CraftingRecipeSlot recipeSlot = slot.GetComponent<CraftingRecipeSlot>();
            recipeSlot.Initialize(craftingManager.availableRecipes.recipes[i], inventory);
        }
    }
}

