using UnityEngine;

public class Resource : ItemBase
{
    public const int STACKSIZE = 64;

    public Resource(ResourceData resourceData, int stacksize = 1) : base(resourceData)
    {
        amount = resourceData.Amount;
    }

    private int amount;
    public int Amount { get { return amount; } set { amount = value; } }
}