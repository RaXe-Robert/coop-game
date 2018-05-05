using System;
using System.Linq;
using UnityEngine;

public class ObjectMapGenerator
{
    public static ObjectMap GenerateObjectMap(int size, ObjectMapSettings settings, Vector2 sampleCenter)
    {
        float[,] values = Noise.GenerateNoiseMap(size, size, settings.NoiseSettings, sampleCenter);
        
        float minValue = float.MaxValue;
        float maxValue = float.MinValue;
        
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                //if (values[x, y] <= density)
                //    values[x, y] = 1f;

                if (values[x, y] > maxValue)
                    maxValue = values[x, y];
                if (values[x, y] < minValue)
                    minValue = values[x, y];
            }
        }
        return new ObjectMap(values, minValue, maxValue, settings);
    }
}

public struct ObjectMap
{
    public readonly float[,] Values;
    public readonly float MinValue;
    public readonly float MaxValue;

    public readonly ObjectMapSettings Settings;

    public ObjectMap(float[,] values, float minValue, float maxValue, ObjectMapSettings settings)
    {
        this.Values = values;
        this.MinValue = minValue;
        this.MaxValue = maxValue;
        this.Settings = settings;
    }
}


