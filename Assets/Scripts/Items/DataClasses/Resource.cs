using UnityEngine;

public class Resource : Item
{
    public Resource(ResourceData resourceData) : base(resourceData)
    {
        stackSize = resourceData.StackSize;
    }

    private int stackSize;
    public int StackSize { get { return stackSize; } set { stackSize = value; } }
}