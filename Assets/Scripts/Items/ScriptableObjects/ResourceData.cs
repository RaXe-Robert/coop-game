using UnityEngine;

[CreateAssetMenu(fileName = "New Resource", menuName = "Entities/Items/Resource")]
public class ResourceData : ScriptableItemData
{
    public override Item InitializeItem()
    {
        return new Resource(this);
    }
}


