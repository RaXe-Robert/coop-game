using UnityEngine;
using System.Collections;

public class DataMapGenerator
{
    public static DataMap GenerateDataMap(int size, HeightMapSettings heightMapSettings, BiomeMapSettings biomeMapSettings, ResourceMapSettings objectMapSettings, Vector2 sampleCenter)
    {
        HeightMap heightMap = HeightMapGenerator.GenerateHeightMap(size, heightMapSettings, sampleCenter);
        BiomeMap biomeMap = BiomeMapGenerator.GenerateBiomeMap(size - 2, biomeMapSettings, sampleCenter);
        ResourceMap resourceMap = ResourceMapGenerator.GenerateResourceMap(size - 2, objectMapSettings, sampleCenter);

        return new DataMap(size - 2, heightMap, biomeMap, resourceMap);
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

    public float GetHeight(int x, int z)
    {
        // These offsets are needed because our mesh has more vertices than that are actually shown on the terrain. 
        // This is because those extra vertices are used in Normal calculations. Only needed for the heightmap.
        return HeightMap.Values[x + 1, z + 1];
    }

    public HeightMapLayer GetLayer(int x, int z)
    {
        return HeightMap.GetLayer(x + 1, z + 1);
    }

    public Biome GetBiome(int x, int z)
    {
        return BiomeMap.GetBiome(x,z);
    }

}
