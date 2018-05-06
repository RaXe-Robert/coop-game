using UnityEngine;

[CreateAssetMenu()]
public class BiomeMapSettings : UpdatableData
{
    public NoiseSettings NoiseSettings;

    public Biome[] Biomes;

#if UNITY_EDITOR

    protected override void OnValidate()
    {
        NoiseSettings.ValidateValues();

        base.OnValidate();
    }

#endif

}

[System.Serializable]
public class Biome
{
    [SerializeField]
    private string name;

    [SerializeField]
    private Color color;

    [SerializeField]
    [Range(0, 1)]
    private float resourceDensity;

    public string Name => name;
    public Color Color => color;
    public float ResourceDensity => 1f - resourceDensity;
}
