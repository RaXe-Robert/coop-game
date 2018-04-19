using UnityEngine;

public abstract class BuildableBase : EntityBase
{
    public bool Recoverable { get; }
    public bool SnapToGrid { get; }

    public BuildableBase(ScriptableBuildableData buildableData) : base(buildableData)
    {
        Recoverable = buildableData.Recoverable;
        SnapToGrid = buildableData.SnapToGrid;
    }
}