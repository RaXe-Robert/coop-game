using UnityEngine;

[CreateAssetMenu(fileName = "New Resource", menuName = "Entities/Items/Resource")]
public class ResourceData : ScriptableItemData
{
    public override EntityBase InitializeEntity()
    {
        return new Resource(this);
    }
}


