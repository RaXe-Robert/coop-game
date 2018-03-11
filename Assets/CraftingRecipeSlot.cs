using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftingRecipeSlot : MonoBehaviour {

    [SerializeField] private GameObject requiredItemPrefab;
    [SerializeField] private Transform requiredItems;
    [SerializeField] private Image recipeResultImage;
    [SerializeField] private Text recipeResultText;

    public CraftingRecipe craftingRecipe;

    private int amountToCraft = 1;
    private Inventory inventory;
    private InputField amountText;

    public void Initialize(CraftingRecipe recipe, Inventory inventory)
    {
        amountText = GetComponentInChildren<InputField>();

        craftingRecipe = recipe;
        this.inventory = inventory;

        //Set result
        recipeResultImage.sprite = craftingRecipe.resultItem.item.Sprite;
        recipeResultText.text = craftingRecipe.resultItem.item.name;

        UpdateRequiredItems();
    }

    private void UpdateRequiredItems()
    {
        for (int i = 0; i < craftingRecipe.requiredItems.Length; i++)
        {
            var go = Instantiate(requiredItemPrefab, requiredItems);
            go.GetComponent<Image>().sprite = craftingRecipe.requiredItems[i].item.Sprite;
            go.GetComponentInChildren<Text>().text = $"x / {craftingRecipe.requiredItems[i].Amount * amountToCraft}";
        }
    }

    private void UpdateAmountText()
    {
        amountText.text = amountToCraft.ToString();
    }

    public void SetAmount(string amount)
    {
        amountToCraft = int.Parse(amount);
        UpdateAmountText();
        UpdateRequiredItems();
    }

    public void OnClick_IncreaseAmount()
    {
        SetAmount((amountToCraft + 1).ToString());
    }

    public void OnClick_DecreaseAmount()
    {
        SetAmount((amountToCraft - 1).ToString());
    }

    public void OnClick_MaxCrafts()
    {
        //Set amountToCraft to maximum amount that is possible based on inventory items.
    }

    public void OnClick_Craft()
    {
        CraftingRecipe a = craftingRecipe;
        a.amountToCraft = amountToCraft;
        FindObjectOfType<CraftingManager>().AddToQueue(a);
    }
}
