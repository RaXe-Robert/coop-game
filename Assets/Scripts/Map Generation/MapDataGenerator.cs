using UnityEngine;
using System.Collections;

public class MapDataGenerator : MonoBehaviour
{
    public static MapData GenerateDataMap(int size, HeightMapSettings heightMapSettings, BiomeMapSettings biomeMapSettings, Vector2 sampleCenter)
    {
        BiomeMap biomeMap = BiomeMapGenerator.GenerateBiomeMap(size - 3, biomeMapSettings, sampleCenter);
        HeightMap heightMap = HeightMapGenerator.GenerateHeightMap(size, heightMapSettings, sampleCenter);

        return new MapData(biomeMap, heightMap);
    }
}

public struct MapData
{
    public readonly BiomeMap BiomeMap;
    public readonly HeightMap HeightMap;

    public MapData(BiomeMap biomeMap, HeightMap heightMap)
    {
        this.BiomeMap = biomeMap;
        this.HeightMap = heightMap;
    }
}
