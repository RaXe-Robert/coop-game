using UnityEngine;

public class ResourceMapGenerator
{
    public static ResourceMap GenerateResourceMap(int size, ResourceMapSettings settings, Vector2 sampleCenter)
    {
        float[,] values = Noise.GenerateNoiseMap(size, size, settings.NoiseSettings, sampleCenter);
        
        float minValue = float.MaxValue;
        float maxValue = float.MinValue;
        
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                if (values[x, y] > maxValue)
                    maxValue = values[x, y];
                if (values[x, y] < minValue)
                    minValue = values[x, y];
            }
        }
        return new ResourceMap(values, minValue, maxValue, settings);
    }
}

public struct ResourceMap
{
    public readonly float[,] Values;
    public readonly float MinValue;
    public readonly float MaxValue;

    public readonly ResourceMapSettings Settings;

    public ResourceMap(float[,] values, float minValue, float maxValue, ResourceMapSettings settings)
    {
        this.Values = values;
        this.MinValue = minValue;
        this.MaxValue = maxValue;
        this.Settings = settings;
    }

    public int GetResourceIndex(int x, int z, int numOfResources)
    {
        int resourceSeed = System.Convert.ToInt32(Values[x, z] * 100f);
        return (new System.Random(resourceSeed)).Next(0, numOfResources);
    }
}


