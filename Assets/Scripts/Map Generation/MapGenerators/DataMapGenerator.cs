using UnityEngine;
using System.Collections;

public class DataMapGenerator
{
    public static DataMap GenerateDataMap(int size, HeightMapSettings heightMapSettings, BiomeMapSettings biomeMapSettings, ResourceMapSettings objectMapSettings, Vector2 sampleCenter)
    {
        HeightMap heightMap = HeightMapGenerator.GenerateHeightMap(size, heightMapSettings, sampleCenter);
        BiomeMap biomeMap = BiomeMapGenerator.GenerateBiomeMap(size - 3, biomeMapSettings, sampleCenter);
        ResourceMap resourceMap = ResourceMapGenerator.GenerateResourceMap(size - 3, objectMapSettings, sampleCenter);

        return new DataMap(size - 3, heightMap, biomeMap, resourceMap);
    }
}

public struct DataMap
{
    public readonly int UniformSize;

    public readonly HeightMap HeightMap;
    public readonly BiomeMap BiomeMap;
    public readonly ResourceMap ResourceMap;

    public DataMap(int uniformSize, HeightMap heightMap, BiomeMap biomeMap, ResourceMap resourceMap)
    {
        this.UniformSize = uniformSize;

        this.HeightMap = heightMap;
        this.BiomeMap = biomeMap;
        this.ResourceMap = resourceMap;
    }
}
