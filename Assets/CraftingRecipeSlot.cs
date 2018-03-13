using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftingRecipeSlot : MonoBehaviour {

    [SerializeField] private GameObject requiredItemPrefab;
    [SerializeField] private Transform requiredItems;
    [SerializeField] private Image recipeResultImage;
    [SerializeField] private Text recipeResultText;
    [SerializeField] private Text craftingTimeText;

    public CraftingRecipe craftingRecipe;

    private CraftingManager craftingManager;
    private int amountToCraft = 1;
    private Inventory inventory;
    private InputField amountText;

    public void Initialize(CraftingRecipe recipe, Inventory inventory)
    {
        craftingManager = FindObjectOfType<CraftingManager>();
        amountText = GetComponentInChildren<InputField>();

        craftingRecipe = recipe;
        this.inventory = inventory;

        //Set result
        recipeResultImage.sprite = craftingRecipe.resultItem.item.Sprite;
        recipeResultText.text = craftingRecipe.resultItem.item.name;
        craftingTimeText.text = $"Crafting Time: {craftingRecipe.craftingTime.ToString()}s";

        InitializeRequiredItems();
    }

    private void InitializeRequiredItems()
    {
        for (int i = 0; i < craftingRecipe.requiredItems.Length; i++)
        {
            var go = Instantiate(requiredItemPrefab, requiredItems);
            go.GetComponent<Image>().sprite = craftingRecipe.requiredItems[i].item.Sprite;
            go.GetComponentInChildren<Text>().text = $"x / {craftingRecipe.requiredItems[i].amount * amountToCraft}";
        }
    }

    private void UpdateRequiredItems()
    {
        for (int i = 0; i < craftingRecipe.requiredItems.Length; i++)
        {
            requiredItems.GetChild(i).gameObject.GetComponentInChildren<Text>().text = $"x / {craftingRecipe.requiredItems[i].amount * amountToCraft}";
        }
    }

    private void UpdateAmountText()
    {
        amountText.text = amountToCraft.ToString();
    }

    public void SetAmount(string amount)
    {
        amountToCraft = int.Parse(amount);
        if (amountToCraft <= 0)
            amountToCraft = 1;
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
        //TODO: check if this results in bugs.
        craftingRecipe.amountToCraft = amountToCraft;
        craftingManager.AddToQueue(craftingRecipe);
    }
}
