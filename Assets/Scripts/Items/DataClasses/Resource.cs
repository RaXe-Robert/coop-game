using UnityEngine;

public class Resource : Item
{
    public const int STACKSIZE = 64;

    public Resource(ResourceData resourceData) : base(resourceData)
    {
        amount = resourceData.Amount;
    }

    private int amount;
    public int Amount { get { return amount; } set { amount = value; } }
}