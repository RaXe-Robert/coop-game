using UnityEngine;

public abstract class ScriptableItemData : ScriptableObject
{
    [SerializeField] private string id;
    [SerializeField] private string itemName;
    [SerializeField] private Sprite sprite;
    [SerializeField] private GameObject model;
    [SerializeField] private string description;
    [SerializeField] private float burningTime;
    private int stackSize = 1;

    //No capital to override the existing Object.name
    public new string name { get { return itemName; } }
    public Sprite Sprite { get { return sprite; } }
    public GameObject Model { get { return model; } }
    public int StackSize { get { return stackSize; } }
    public string Description { get { return description; } }
    public string Id { get { return id; } }
    public float BurningTime { get { return burningTime; } }

    public abstract Item InitializeItem();
}
