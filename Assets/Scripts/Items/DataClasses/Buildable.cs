using UnityEngine;

public class Buildable : ItemBase
{
    public bool Recoverable { get; }
    public bool SnapToGrid { get; }

    public Buildable(BuildableData buildableData) : base(buildableData)
    {
        Recoverable = buildableData.Recoverable;
        SnapToGrid = buildableData.SnapToGrid;
    }
}