using System;
using UnityEngine;

[Serializable]
public class CraftingRecipe
{
    public CraftingEntity[] requiredEntities;
    public CraftingEntity result;
    [HideInInspector]
    public int amountToCraft;
    public float craftingTime;
}