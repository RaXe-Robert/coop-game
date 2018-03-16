using System.Collections.Generic;
using UnityEngine;

public class ItemData : ScriptableObject
{
    [SerializeField] private string itemName;
    [SerializeField] private Sprite sprite;
    [SerializeField] private GameObject model;
    [SerializeField] private string description;
    [SerializeField] private bool isConsumable;
    [SerializeField] private List<ScriptableStatusEffect> onConsumedEffects;
    [SerializeField] private int id;

    //No capital to override the existing Object.name
    public new string name { get { return itemName; } }
    public Sprite Sprite { get { return sprite; } }
    public GameObject Model { get { return model; } }
    public string Description { get { return description; } }
    public bool IsConsumable { get { return isConsumable; } }
    public List<ScriptableStatusEffect> OnConsumedEffects { get { return onConsumedEffects; } }
    public int Id { get { return id; } }
}

