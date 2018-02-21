using UnityEngine;

public class Item : ScriptableObject
{
    [SerializeField] private string itemName;
    [SerializeField] private Sprite sprite;
    [SerializeField] private GameObject model;
    [SerializeField] private string description;

    //No capital to override the existing Object.name
    public new string name { get { return itemName; } }
    public Sprite Sprite { get { return sprite; } }
    public GameObject Model { get { return model; } }
    public string Description { get { return description; } }
}
