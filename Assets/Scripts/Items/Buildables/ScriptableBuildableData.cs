using UnityEngine;

[CreateAssetMenu(fileName = "New buildable", menuName = "Items/Buildable")]
public class ScriptableBuildableData : ScriptableItemData
{
    [Header("Building Settings")]
    [Tooltip("Can this buildable be picked up after it's placed?")]
    [SerializeField] private bool recoverable;
    public bool Recoverable { get { return recoverable; } }

    [Tooltip("Should be build on a grid?")]
    [SerializeField] private bool snapToGrid;
    public bool SnapToGrid { get { return snapToGrid; } }

    [Tooltip("Prefab to spawn")]
    [SerializeField] private GameObject prefabToSpawn;
    public GameObject PrefabToSpawn { get { return prefabToSpawn; } }

    public override Item InitializeItem()
    {
        return new BuildableBase(this);
    }
}


