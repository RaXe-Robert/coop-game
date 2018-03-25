using UnityEngine;
using System.Collections.Generic;

public class ItemBase
{
    public const int MAXSTACKSIZE = 64;

    public ItemBase(ScriptableItemData itemData)
    {
        itemName = itemData.name;
        sprite = itemData.Sprite;
        model = itemData.Model;
        description = itemData.Description;
        isConsumable = itemData.IsConsumable;
        onConsumedEffects = itemData.OnConsumedEffects;
        id = itemData.Id;
    }

    private string itemName;
    private Sprite sprite;
    private GameObject model;
    private int stackSize;
    private string description;
    private bool isConsumable;
    private List<ScriptableStatusEffectData> onConsumedEffects;
    private int id;

    public string Name { get { return itemName; } }
    public Sprite Sprite { get { return sprite; } }
    public GameObject Model { get { return model; } }
    public int StackSize { get { return stackSize; } set { stackSize = value; } }
    public string Description { get { return description; } }
    public bool IsConsumable { get { return isConsumable; } }
    public List<ScriptableStatusEffectData> OnConsumedEffects { get { return onConsumedEffects; } }
    public int Id { get { return id; } }
    public bool Equippable { get { return GetType() == typeof(Armor) || GetType() == typeof(Tool) || GetType() == typeof(Weapon); } }
}
