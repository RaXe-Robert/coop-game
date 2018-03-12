using UnityEngine;

[CreateAssetMenu(fileName = "New Resource", menuName = "Items/Resource")]
public class ResourceData : ItemData
{
    [SerializeField] private int stackSize;
    public int StackSize { get { return stackSize; } set { stackSize = value; } }
}


