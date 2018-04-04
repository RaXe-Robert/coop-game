using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class ScriptableItemData : ScriptableObject
{
    [SerializeField] private string itemName;
    [SerializeField] private Sprite sprite;
    [SerializeField] private GameObject model;
    [SerializeField] private string description;
    [SerializeField] private bool isConsumable;
    [SerializeField] private List<ScriptableStatusEffectData> onConsumedEffects;
    [SerializeField] private int id;
    private int stackSize = 1;

    //No capital to override the existing Object.name
    public new string name { get { return itemName; } }
    public Sprite Sprite { get { return sprite; } }
    public GameObject Model { get { return model; } }
    public int StackSize { get { return stackSize; } }
    public string Description { get { return description; } }
    public bool IsConsumable { get { return isConsumable; } }
    public List<ScriptableStatusEffectData> OnConsumedEffects { get { return onConsumedEffects; } }
    public int Id { get { return id; } }

    public abstract ItemBase InitializeItem();
}
