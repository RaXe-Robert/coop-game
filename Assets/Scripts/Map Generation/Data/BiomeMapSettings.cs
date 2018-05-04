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
    public string name;
    public Color color;
}
