using UnityEngine;

[CreateAssetMenu(fileName = "New Buildable", menuName = "Items/Builable")]
public class BuildableData : ScriptableItemData
{
    [Tooltip("Can this buildable be picked up after it's placed?")]
    [SerializeField] private float recoverable;
    public float Recoverable { get { return recoverable; } }

    public override ItemBase InitializeItem()
    {
        return new Buildable(this);
    }
}


