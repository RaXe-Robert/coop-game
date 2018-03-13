using UnityEngine;

[CreateAssetMenu(fileName = "New Resource", menuName = "Items/Resource")]
public class ResourceData : ItemData
{
    [SerializeField] private int amount;
    public int Amount { get { return amount; } set { amount = value; } }
}


