using UnityEngine;
using System.Collections.Generic;

public class ItemBase
{
    public const int MAXSTACKSIZE = 64;

    public ItemBase(ScriptableItemData itemData)
    {
        Name = itemData.name;
        Sprite = itemData.Sprite;
        Model = itemData.Model;
        Description = itemData.Description;
        IsConsumable = itemData.IsConsumable;
        OnConsumedEffects = itemData.OnConsumedEffects;
        Id = itemData.Id;
    }
    
    public string Name { get; }
    public Sprite Sprite { get; }
    public GameObject Model { get; }
    public string Description { get; }
    public bool IsConsumable { get; }
    public List<ScriptableStatusEffectData> OnConsumedEffects { get; }
    public int Id { get; }

    public int StackSize { get; set; }
    public bool Equippable { get { return GetType() == typeof(Armor) || GetType() == typeof(Tool) || GetType() == typeof(Weapon); } }
}
