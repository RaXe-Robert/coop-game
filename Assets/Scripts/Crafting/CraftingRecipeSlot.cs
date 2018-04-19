using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftingRecipeSlot : MonoBehaviour {

    [SerializeField] private GameObject requiredEntityPrefab;
    [SerializeField] private Transform requiredEntities;
    [SerializeField] private Image recipeResultImage;
    [SerializeField] private Text recipeResultText;
    [SerializeField] private Text craftingTimeText;

    public CraftingRecipe craftingRecipe;

    private CraftingManager craftingManager;
    private int amountToCraft;
    private Inventory inventory;
    private InputField amountText;

    public void Initialize(CraftingRecipe recipe, Inventory inventory)
    {
        craftingManager = FindObjectOfType<CraftingManager>();
        amountText = GetComponentInChildren<InputField>();
        amountToCraft = 1;

        craftingRecipe = recipe;
        this.inventory = inventory;
        inventory.OnEntityChangedCallback += UpdateRequiredEntities;

        //Set result
        recipeResultImage.sprite = craftingRecipe.result.entity.Sprite;
        recipeResultText.text = craftingRecipe.result.entity.name;
        craftingTimeText.text = $"Crafting Time: {craftingRecipe.craftingTime.ToString()}s";

        InitializeRequiredEntities();
    }

    private void InitializeRequiredEntities()
    {
        for (int i = 0; i < craftingRecipe.requiredEntities.Length; i++)
        {
            var go = Instantiate(requiredEntityPrefab, requiredEntities);
            go.GetComponent<Image>().sprite = craftingRecipe.requiredEntities[i].entity.Sprite;
            go.GetComponentInChildren<Text>().text = $"{inventory.GetEntityAmountById(craftingRecipe.requiredEntities[i].entity.Id)} / {craftingRecipe.requiredEntities[i].amount * amountToCraft}";
        }
    }

    private void UpdateRequiredEntities()
    {
        for (int i = 0; i < craftingRecipe.requiredEntities.Length; i++)
        {
            requiredEntities.GetChild(i).gameObject.GetComponentInChildren<Text>().text = $"{inventory.GetEntityAmountById(craftingRecipe.requiredEntities[i].entity.Id)} / {craftingRecipe.requiredEntities[i].amount * amountToCraft}";
        }
    }

    private void UpdateAmountText()
    {
        amountText.text = amountToCraft.ToString();
    }

    public void SetAmount(string amount)
    {
        amountToCraft = int.Parse(amount) <= 0 ? 1 : int.Parse(amount);
        UpdateAmountText();
        UpdateRequiredEntities();
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
        SetAmount(inventory.GetMaxCrafts(craftingRecipe).ToString());
    }

    public void OnClick_ResetAmount()
    {
        SetAmount("1");
    }

    public void OnClick_Craft()
    {
        //TODO: check if this results in bugs.
        var a = craftingRecipe;
        a.amountToCraft = amountToCraft;
        craftingManager.AddToQueue(a);
    }
}
