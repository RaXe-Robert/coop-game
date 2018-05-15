﻿using System.Collections.Generic;
using UnityEngine;

public class ResourceMapGenerator
{
    public static ResourceMap GenerateResourceMap(int size, ResourceMapSettings settings, Vector2 sampleCenter)
    {
        float[,] values = Noise.GenerateNoiseMap(size, size, settings.NoiseSettings, sampleCenter);
        List<ObjectPoint> resourcePoints = new List<ObjectPoint>();

        float minValue = float.MaxValue;
        float maxValue = float.MinValue;
        
        for (int x = 0; x < size; x++)
        {
            for (int z = 0; z < size; z++)
            {
                if (values[x, z] >= settings.DensityThreshold)
                {
                    // Get a random world resource with a seed that is created from the data at this x,z position.
                    int resourceSeed = System.Convert.ToInt32(x + z + values[x, z] * 100f);
                    int resourceIndex = (new System.Random(resourceSeed)).Next(0, settings.WorldResourceEntries.Length);

                    resourcePoints.Add(new ObjectPoint(x, z, resourceIndex));
                }

                if (values[x, z] > maxValue)
                    maxValue = values[x, z];
                if (values[x, z] < minValue)
                    minValue = values[x, z];
            }
        }
        return new ResourceMap(values, minValue, maxValue, resourcePoints.ToArray(), settings);
    }
}

public struct ResourceMap
{
    public readonly float[,] Values;
    public readonly float MinValue;
    public readonly float MaxValue;

    public readonly ObjectPoint[] ObjectPoints;

    public readonly ResourceMapSettings Settings;

    public ResourceMap(float[,] values, float minValue, float maxValue, ObjectPoint[] objectPoints, ResourceMapSettings settings)
    {
        this.Values = values;
        this.MinValue = minValue;
        this.MaxValue = maxValue;
        this.ObjectPoints = objectPoints;
        this.Settings = settings;
    }
}

[System.Serializable]
public struct ObjectPoint
{
    public readonly int IndexX; 
    public readonly int IndexZ;
    public readonly int WorldResourcePrefabID;

    public ObjectPoint(int x, int z, int worldResourcePrefabID)
    {
        this.IndexX = x;
        this.IndexZ = z;
        this.WorldResourcePrefabID = worldResourcePrefabID;
    }
}


