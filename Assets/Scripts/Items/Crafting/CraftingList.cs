using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[Serializable][CreateAssetMenu(fileName = "CraftingList", menuName = "Crafting")]
public class CraftingList : ScriptableObject
{
    public List<CraftingRecipe> recipes;
}