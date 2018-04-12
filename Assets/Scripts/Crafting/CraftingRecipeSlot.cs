using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftingRecipeSlot : MonoBehaviour
{

    [SerializeField] private GameObject requiredItemPrefab;
    [SerializeField] private Transform requiredItems;
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
        inventory.OnItemChangedCallback += UpdateRequiredItems;

        if (ValidateRecipe())
        {
            //Set result
            recipeResultImage.sprite = craftingRecipe.result.item.Sprite;
            recipeResultText.text = craftingRecipe.result.item.name;
            craftingTimeText.text = $"Crafting Time: {craftingRecipe.craftingTime.ToString()}s";

            InitializeRequiredItems();
        }
    }

    //Logging if something is wrong with the crafting recipe    
    private bool ValidateRecipe()
    {
        if (craftingRecipe.result.item == null || craftingRecipe.result.amount <= 0)
        {
            Debug.LogError($"There is a crafting recipe without a result or the result amount is invalid");
            Destroy(gameObject);
            return false;
        }
        foreach (var item in craftingRecipe.requiredItems)
        {
            if (item.item == null)
            {
                Debug.LogError($"{craftingRecipe.result.item.name} crafting recipe is missing some data");
                Destroy(gameObject);
                return false;
            }
            if (item.amount <= 0)
            {
                Debug.LogError($"{craftingRecipe.result.item.name} recipe has a required item with an invalid amount");
                Destroy(gameObject);
                return false;
            }
        }
        return true;
    }

    private void InitializeRequiredItems()
    {
        for (int i = 0; i < craftingRecipe.requiredItems.Count; i++)
        {
            var go = Instantiate(requiredItemPrefab, requiredItems);

            go.GetComponent<Image>().sprite = craftingRecipe.requiredItems[i].item.Sprite;
            go.GetComponentInChildren<Text>().text = $"{inventory.GetItemAmountById(craftingRecipe.requiredItems[i].item.Id)} / {craftingRecipe.requiredItems[i].amount * amountToCraft}";
        }
    }

    private void UpdateRequiredItems()
    {
        for (int i = 0; i < craftingRecipe.requiredItems.Count; i++)
        {
            //Debug.Log(inventory.GetItemAmountById(inventory.GetItemAmountById(craftingRecipe.requiredItems[i].item.Id)));
            requiredItems.GetChild(i).gameObject.GetComponentInChildren<Text>().text = $"{inventory.GetItemAmountById(craftingRecipe.requiredItems[i].item.Id)} / {craftingRecipe.requiredItems[i].amount * amountToCraft}";
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
