using System.Collections.Generic;
using UnityEngine;

public class GenericItemData : ScriptableItemData
{
    public override ItemBase InitializeItem()
    {
        return new GenericItem(this);
    }
}

