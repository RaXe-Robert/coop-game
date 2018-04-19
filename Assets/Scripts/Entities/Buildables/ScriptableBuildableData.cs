using UnityEngine;

public abstract class ScriptableBuildableData : ScriptableEntityData
{
    [Header("Building Settings")]
    [Tooltip("Can this buildable be picked up after it's placed?")]
    [SerializeField] private bool recoverable;
    public bool Recoverable { get { return recoverable; } }

    [Tooltip("Should be build on a grid?")]
    [SerializeField] private bool snapToGrid;
    public bool SnapToGrid { get { return snapToGrid; } }
}


