using UnityEngine;

public abstract class ScriptableEntityData : ScriptableObject
{
    [SerializeField] private string entityName;
    [SerializeField] private Sprite sprite;
    [SerializeField] private GameObject model;
    [SerializeField] private string description;
    [SerializeField] private int id;
    private int stackSize = 1;

    //No capital to override the existing Object.name
    public new string name { get { return entityName; } }
    public Sprite Sprite { get { return sprite; } }
    public GameObject Model { get { return model; } }
    public int StackSize { get { return stackSize; } }
    public string Description { get { return description; } }
    public int Id { get { return id; } }

    public abstract EntityBase InitializeEntity();
}
