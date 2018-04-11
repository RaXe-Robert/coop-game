using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CraftingRecipe
{
    public List<CraftingItem> requiredItems;
    public CraftingItem result;
    [HideInInspector]
    public int amountToCraft;
    public float craftingTime;
}