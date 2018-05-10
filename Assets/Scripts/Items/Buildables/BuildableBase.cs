using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BuildableBase : Item
{
    public bool Recoverable { get; }
    public bool SnapToGrid { get; }
    public GameObject PrefabToSpawn { get; }

    public BuildableBase(ScriptableBuildableData buildableData) : base(buildableData)
    {
        Recoverable = buildableData.Recoverable;
        SnapToGrid = buildableData.SnapToGrid;
        PrefabToSpawn = buildableData.PrefabToSpawn;
    }
}