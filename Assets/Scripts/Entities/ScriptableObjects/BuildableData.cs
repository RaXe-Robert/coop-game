using UnityEngine;

[CreateAssetMenu(fileName = "New Buildable", menuName = "Entities/Items/Builable")]
public class BuildableData : ScriptableEntityData
{
    [Header("Building Settings")]

    [Tooltip("Can this buildable be picked up after it's placed?")]
    [SerializeField] private bool recoverable;
    public bool Recoverable { get { return recoverable; } }

    [Tooltip("Should be build on a grid?")]
    [SerializeField] private bool snapToGrid;
    public bool SnapToGrid { get { return snapToGrid; } }

    public override EntityBase InitializeEntity()
    {
        return new Buildable(this);
    }
}


