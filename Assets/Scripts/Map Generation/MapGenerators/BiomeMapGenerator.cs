using UnityEngine;
using System.Collections;

public static class BiomeMapGenerator
{
    public static BiomeMap GenerateBiomeMap(int size, BiomeMapSettings settings, Vector2 sampleCenter)
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
        return new BiomeMap(values, minValue, maxValue, settings);
    }
}

public struct BiomeMap
{
    public readonly float[,] Values;

    public readonly float MinValue;
    public readonly float MaxValue;

    public readonly BiomeMapSettings Settings;
    public int NumOfBiomes => Settings.Biomes.Length;

    public BiomeMap(float[,] values, float minValue, float maxValue, BiomeMapSettings settings)
    {
        this.Values = values;
        this.MinValue = minValue;
        this.MaxValue = maxValue;
        this.Settings = settings;
    }

    public Biome GetBiome(int x, int z)
    {
        Biome biome = Settings.Biomes[0];
        
        if (NumOfBiomes > 2)
        {
            // Loop through all other biomes
            for (int biomeIndex = 1; biomeIndex < NumOfBiomes; biomeIndex++)
            {
                if (Values[x, z] > (MaxValue - MinValue) / NumOfBiomes * biomeIndex)
                {
                    biome = Settings.Biomes[biomeIndex];
                }
            }
        }

        return biome;
    }
}
