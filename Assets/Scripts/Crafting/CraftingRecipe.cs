using System;
using UnityEngine;

[Serializable]
public class CraftingRecipe
{
    public CraftingItem[] requiredItems;
    public CraftingItem result;
    [HideInInspector]
    public int amountToCraft;
    public float craftingTime;
}