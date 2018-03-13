using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class CraftingRecipe
{
    public CraftingItem[] requiredItems;
    public CraftingItem resultItem;
    [HideInInspector]
    public int amountToCraft;
    public float craftingTime;
}