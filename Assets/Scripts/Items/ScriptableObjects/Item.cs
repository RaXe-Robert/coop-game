using UnityEngine;

public class Item : ScriptableObject
{
    [SerializeField] private string itemName;
    [SerializeField] private Sprite sprite;
    [SerializeField] private GameObject model;
    [SerializeField] private string description;
    [SerializeField] private int id;

    //No capital to override the existing Object.name
    public new string name { get { return itemName; } }
    public Sprite Sprite { get { return sprite; } }
    public GameObject Model { get { return model; } }
    public string Description { get { return description; } }
    public int Id { get; private set; }

    public static Item CreateInstance(Item itemData)
    {
        var item = CreateInstance<Item>();
        item.itemName = itemData.name;
        item.sprite = itemData.Sprite;
        item.model = itemData.Model;
        item.description = itemData.Description;
        item.id = itemData.Id;
        return item;
    }
}
