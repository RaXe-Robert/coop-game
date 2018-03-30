using UnityEngine;

[CreateAssetMenu(fileName = "New Buildable", menuName = "Items/Builable")]
public class BuildableData : ScriptableItemData
{
    [Header("Building Settings")]
    [Tooltip("Can this buildable be picked up after it's placed?")]
    [SerializeField] private bool recoverable;
    public bool Recoverable { get { return recoverable; } }

    public override ItemBase InitializeItem()
    {
        return new Buildable(this);
    }
}


