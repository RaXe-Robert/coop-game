using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

public class CraftingItemTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string ItemName;
    public int Amount;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (ItemName == null)
            return;

        string text = Amount > 0 ? $"{ItemName} ({Amount})" : ItemName;
        Tooltip.Instance.Show(text);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Tooltip.Instance.Hide();
    }
}