using UnityEngine;

[CreateAssetMenu(fileName = "New Resource", menuName = "Items/Resource")]
public class ResourceData : ScriptableItemData
{
    [SerializeField] private int amount;
    public int Amount { get { return amount; } set { amount = value; } }

    public override ItemBase InitializeItem()
    {
        return new Resource(this);
    }
}


