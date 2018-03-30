using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CraftingTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    [SerializeField] private Image targetImage;
    private ScriptableItemData item;
    private float craftingTime;

    public void Initialize(CraftingRecipe recipe)
    {
        item = recipe.result.item;
        craftingTime = recipe.craftingTime;
        targetImage.sprite = item.Sprite;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (item != null)
            Tooltip.Instance.Show(item.name, craftingTime.ToString("F0"));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Tooltip.Instance.Hide();
    }
}
