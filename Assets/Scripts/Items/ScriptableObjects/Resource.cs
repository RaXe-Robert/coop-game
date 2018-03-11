using UnityEngine;

[CreateAssetMenu(fileName = "New Resource", menuName = "Items/Resource")]
public class Resource : Item
{
    [SerializeField] private int stackSize;
    public int StackSize { get { return stackSize; } set { stackSize = value; } }

    public static Resource CreateResource(Resource resourceData)
    {
        Resource item = CreateInstance(resourceData) as Resource;
        item.stackSize = resourceData.StackSize;
        return item;
    }
}

