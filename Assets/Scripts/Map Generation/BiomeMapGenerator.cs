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
    public readonly BiomeMapSettings settings;

    public readonly float[,] values;
    public readonly int numOfBiomes;

    public readonly float minValue;
    public readonly float maxValue;

    public BiomeMap(BiomeMapSettings settings, float[,] values, int numBiomes, float minValue, float maxValue)
    {
        this.settings = settings;
        this.values = values;
        this.numOfBiomes = numBiomes;
        this.minValue = minValue;
        this.maxValue = maxValue;
    }
}
