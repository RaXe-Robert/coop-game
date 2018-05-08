using UnityEngine;

public class EquippableData : ScriptableItemData
{
    public override Item InitializeItem()
    {
        return new Equippable(this);
    }
}