using UnityEngine;
using System.Collections;

public static class BiomeMapGenerator
{
    public static BiomeMap GenerateBiomeMap(int size, BiomeMapSettings settings, Vector2 sampleCenter)
    {
        float[,] values = Noise.GenerateNoiseMap(size, size, settings.NoiseSettings, sampleCenter);

        float minValue = float.MaxValue;
        float maxValue = float.MinValue;

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                values[i, j] *= 1f;

                if (values[i, j] > maxValue)
                    maxValue = values[i, j];
                if (values[i, j] < minValue)
                    minValue = values[i, j];
            }
        }
        return new BiomeMap(settings, values, settings.Biomes.Length, minValue, maxValue);
    }
}

public struct BiomeMap
{
    public readonly BiomeMapSettings Settings;

    public readonly float[,] Values;
    public readonly int NumOfBiomes;

    public readonly float MinValue;
    public readonly float MaxValue;

    public BiomeMap(BiomeMapSettings settings, float[,] values, int numBiomes, float minValue, float maxValue)
    {
        this.Settings = settings;
        this.Values = values;
        this.NumOfBiomes = numBiomes;
        this.MinValue = minValue;
        this.MaxValue = maxValue;
    }

    public Biome GetBiomeFromValue(int x, int z)
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
