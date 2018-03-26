using UnityEngine;

[CreateAssetMenu(fileName = "New Buildable", menuName = "Items/Builable")]
public class BuildableData : ScriptableItemData
{
    public override ItemBase InitializeItem()
    {
        return new Buildable(this);
    }
}


