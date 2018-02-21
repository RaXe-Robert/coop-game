using UnityEngine;

public class Item : ScriptableObject
{
    [SerializeField] private string itemName;
    [SerializeField] private Sprite sprite;
    [SerializeField] private string description;

    //No capital to override the existing Object.name
    public new string name { get { return itemName; } }
    public Sprite Sprite { get { return sprite; } }
    public string Description { get { return description; } }

    public static GameObject CreateWorldObject(Item item, Vector3 position, Transform parent = null)
    {
        GameObject go = new GameObject()
        {
            name = item.name
        };
        go.transform.position = position;
        go.transform.SetParent(parent);
        go.AddComponent<ItemWorldObject>().item = item;

        return go;
    }
}
