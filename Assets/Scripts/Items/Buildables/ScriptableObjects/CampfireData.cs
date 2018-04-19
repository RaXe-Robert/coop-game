using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "New Campfire", menuName = "Entities/Buildables/Campfire")]
public class CampfireData : ScriptableBuildableData
{
    [SerializeField] private GameObject fireParticles;
    
    public GameObject FireParticles => fireParticles;
    
    public override Item InitializeItem()
    {
        return new Campfire(this);
    }
}
