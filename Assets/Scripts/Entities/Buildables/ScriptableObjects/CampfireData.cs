using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "New Campfire", menuName = "Entities/Buildables/Campfire")]
public class CampfireData : ScriptableBuildableData
{
    public override EntityBase InitializeEntity()
    {
        return new Campfire(this);
    }
}
