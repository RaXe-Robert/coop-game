using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CraftingTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    [SerializeField] private Image targetImage;
    private ScriptableEntityData entity;
    private float craftingTime;

    public void Initialize(CraftingRecipe recipe)
    {
        entity = recipe.result.entity;
        craftingTime = recipe.craftingTime;
        targetImage.sprite = entity.Sprite;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (entity != null)
            Tooltip.Instance.Show(entity.name, craftingTime.ToString("F0") + "s");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Tooltip.Instance.Hide();
    }
}
