using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

public abstract class BuildableBase : EntityBase
{
    public bool Recoverable { get; }
    public bool SnapToGrid { get; }

    public List<UnityAction> Actions { get; }

    public BuildableBase(ScriptableBuildableData buildableData) : base(buildableData)
    {
        Recoverable = buildableData.Recoverable;
        SnapToGrid = buildableData.SnapToGrid;

        Actions = new List<UnityAction>();
        Actions.AddRange(InitializeActions());

        if (Recoverable)
            Actions.Add(new UnityAction(Pickup));
    }

    private void Pickup()
    {
        // If null the action will be cancelled
        if (BuildableInteractionMenu.Instance?.Target == null)
            return;

        BuildableInteractionMenu.Instance.Target.DestroyWorldObject();
    }

    protected abstract UnityAction[] InitializeActions();
}