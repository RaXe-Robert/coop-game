using UnityEngine;

[CreateAssetMenu(fileName = "New Resource", menuName = "Items/Resource")]
public class Resource : Item
{
    [SerializeField] private int stackSize;
    public int StackSize { get { return stackSize; } }
}

