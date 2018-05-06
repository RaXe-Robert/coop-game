using UnityEngine;

[CreateAssetMenu()]
public class ResourceMapSettings : UpdatableData
{
    [SerializeField]
    private NoiseSettings noiseSettings;

    [SerializeField]
    private WorldResourceEntry[] worldResourceEntries;

    [SerializeField]
    [Range(0,1)]
    private float densityThreshold;

    public NoiseSettings NoiseSettings => noiseSettings;
    public WorldResourceEntry[] WorldResourceEntries => worldResourceEntries;
    public float DensityThreshold { get { return 1f - densityThreshold; } }

#if UNITY_EDITOR

    protected override void OnValidate()
    {
        NoiseSettings.ValidateValues();

        densityThreshold = Mathf.Clamp(densityThreshold, 0f, 1f);

        base.OnValidate();
    }

#endif
}

[System.Serializable]
public class WorldResourceEntry
{
    [SerializeField]
    private GameObject worldResourcePrefab;

    public GameObject WorldResourcePrefab => worldResourcePrefab;
}