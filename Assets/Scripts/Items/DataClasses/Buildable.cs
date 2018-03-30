using UnityEngine;

public class Buildable : ItemBase
{
    public bool Recoverable { get; }

    public Buildable(BuildableData buildableData) : base(buildableData)
    {
        Recoverable = buildableData.Recoverable;
    }
}