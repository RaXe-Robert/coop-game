using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CraftingRecipe
{
    public List<CraftingEntity> requiredEntities;
    public CraftingEntity result;
    [HideInInspector]
    public int amountToCraft;
    public float craftingTime;
}