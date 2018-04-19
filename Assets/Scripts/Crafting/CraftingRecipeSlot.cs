using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftingRecipeSlot : MonoBehaviour
{
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

        if (ValidateRecipe())
        {
            //Set result
            inventory.OnEntityChangedCallback += UpdateRequiredEntities;
            recipeResultImage.sprite = craftingRecipe.result.entity.Sprite;
            recipeResultText.text = craftingRecipe.result.entity.name;
            craftingTimeText.text = $"Crafting Time: {craftingRecipe.craftingTime.ToString()}s";

            InitializeRequiredEntities();
        }
    }

    //Logging if something is wrong with the crafting recipe    
    private bool ValidateRecipe()
    {
        if (craftingRecipe.result.entity == null || craftingRecipe.result.amount <= 0)
        {
            Debug.LogError($"There is a crafting recipe without a result or the result amount is invalid");
            Destroy(gameObject);
            return false;
        }
        foreach (var craftingEntity in craftingRecipe.requiredEntities)
        {
            if (craftingEntity.entity == null)
            {
                Debug.LogError($"{craftingRecipe.result.entity.name} crafting recipe is missing some data");
                Destroy(gameObject);
                return false;
            }
            if (craftingEntity.amount <= 0)
            {
                Debug.LogError($"{craftingRecipe.result.entity.name} recipe has a required entity with an invalid amount");
                Destroy(gameObject);
                return false;
            }
        }
        return true;
    }

    private void InitializeRequiredEntities()
    {
        for (int i = 0; i < craftingRecipe.requiredEntities.Count; i++)
        {
            var go = Instantiate(requiredEntityPrefab, requiredEntities);

            go.GetComponent<Image>().sprite = craftingRecipe.requiredEntities[i].entity.Sprite;
            go.GetComponentInChildren<Text>().text = $"{inventory.GetEntityAmountById(craftingRecipe.requiredEntities[i].entity.Id)} / {craftingRecipe.requiredEntities[i].amount * amountToCraft}";
        }
    }

    private void UpdateRequiredEntities()
    {
        for (int i = 0; i < craftingRecipe.requiredEntities.Count; i++)
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
