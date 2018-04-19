using UnityEngine;
using System.Collections.Generic;

public class EntityBase
{
    public const int MAXSTACKSIZE = 64;

    public EntityBase(ScriptableEntityData entityData)
    {
        Name = entityData.name;
        Sprite = entityData.Sprite;
        Model = entityData.Model;
        Description = entityData.Description;
        Id = entityData.Id;
    }
    
    public string Name { get; }
    public Sprite Sprite { get; }
    public GameObject Model { get; }
    public string Description { get; }
    public int Id { get; }

    public int StackSize { get; set; }
}
