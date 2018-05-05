using UnityEngine;
using System.Collections;

public class DataMapGenerator
{
    public static DataMap GenerateDataMap(int size, HeightMapSettings heightMapSettings, BiomeMapSettings biomeMapSettings, ObjectMapSettings objectMapSettings, Vector2 sampleCenter)
    {
        HeightMap heightMap = HeightMapGenerator.GenerateHeightMap(size, heightMapSettings, sampleCenter);
        BiomeMap biomeMap = BiomeMapGenerator.GenerateBiomeMap(size - 3, biomeMapSettings, sampleCenter);
        ObjectMap objectMap = ObjectMapGenerator.GenerateObjectMap(size - 3, objectMapSettings, sampleCenter);

        return new DataMap(size - 3, heightMap, biomeMap, objectMap);
    }
}

public struct DataMap
{
    public readonly int UniformSize;

    public readonly HeightMap HeightMap;
    public readonly BiomeMap BiomeMap;
    public readonly ObjectMap ObjectMap;

    public DataMap(int uniformSize, HeightMap heightMap, BiomeMap biomeMap, ObjectMap objectMap)
    {
        this.UniformSize = uniformSize;
        this.HeightMap = heightMap;
        this.BiomeMap = biomeMap;
        this.ObjectMap = objectMap;
    }
}
