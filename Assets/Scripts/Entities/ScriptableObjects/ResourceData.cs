using UnityEngine;

[CreateAssetMenu(fileName = "New Resource", menuName = "Entities/Items/Resource")]
public class ResourceData : ScriptableEntityData
{
    public override EntityBase InitializeEntity()
    {
        return new Resource(this);
    }
}


