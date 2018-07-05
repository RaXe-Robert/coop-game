using UnityEngine;
using System.Collections.Generic;

public class Item
{
    public const int MAXSTACKSIZE = 64;

    public Item(ScriptableItemData itemData)
    {
        Name = itemData.name;
        Sprite = itemData.Sprite;
        Model = itemData.Model;
        Description = itemData.Description;
        Id = itemData.Id;
        BurningTime = itemData.BurningTime;
        MeltingResult = itemData.MeltingResult;
        CookingResult = itemData.CookingResult;
    }
    
    public string Name { get; }
    public Sprite Sprite { get; }
    public GameObject Model { get; }
    public string Description { get; }
    public string Id { get; }
    public float BurningTime { get; }
    public ScriptableItemData MeltingResult { get; }
    public ScriptableItemData CookingResult { get; }
    public int StackSize { get; set; }
}
