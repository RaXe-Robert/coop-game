using System;
using System.Collections.Generic;
using UnityEngine;

public class DataMapGenerator
{
    /// <summary>
    /// Generates a <see cref="DataMap"/> that consists of a heightmap, biomeMap and resourceMap based on a single size. Divides the resourceMap in <see cref="DataMap.ChunkParts"/> based on <see cref="MeshSettings.ChunkPartSizeRoot"/>
    /// </summary>
    /// <param name="terrainChunk">The terrain chunk that requested the DataMap.</param>
    /// <returns></returns>
    public static DataMap GenerateDataMap(MeshSettings meshSettings, HeightMapSettings heightMapSettings, BiomeMapSettings biomeMapSettings, ResourceMapSettings objectMapSettings, TerrainChunk terrainChunk)
    {
        int size = meshSettings.NumVertsPerLine;
        int uniformSize = size - 2;

        HeightMap heightMap = HeightMapGenerator.GenerateHeightMap(size, heightMapSettings, terrainChunk.SampleCenter);
        BiomeMap biomeMap = BiomeMapGenerator.GenerateBiomeMap(uniformSize, biomeMapSettings, terrainChunk.SampleCenter);
        ResourceMap resourceMap = ResourceMapGenerator.GenerateResourceMap(uniformSize, objectMapSettings, terrainChunk.SampleCenter);

        Dictionary<Vector2, ChunkPart> chunkParts = CreateAndFillChunkParts(uniformSize, meshSettings, heightMap, resourceMap, terrainChunk);

        return new DataMap(uniformSize, heightMap, biomeMap, resourceMap, chunkParts);
    }

    private static Dictionary<Vector2, ChunkPart> CreateAndFillChunkParts(int uniformSize, MeshSettings meshSettings, HeightMap heightMap, ResourceMap resourceMap, TerrainChunk terrainChunk)
    {
        Dictionary<Vector2, ChunkPart> chunkParts = new Dictionary<Vector2, ChunkPart>();

        ResourcePoint[] resourcePoints = resourceMap.ResourcePoints;

        int chunkRangeHalf = Mathf.FloorToInt(meshSettings.ChunkPartSizeRoot / 2f); // ONLY WORKS FOR UNEVEN NUMBERS AT THE MOMENT

        for (int x = -chunkRangeHalf; x <= chunkRangeHalf; x++)
        {
            for (int z = -chunkRangeHalf; z <= chunkRangeHalf; z++)
            {
                Vector2 chunkPartCoord = terrainChunk.Coord * meshSettings.ChunkPartSizeRoot + new Vector2(x, z);
                Vector3 partWorldPosition = new Vector3(terrainChunk.Bounds.size.x / meshSettings.ChunkPartSizeRoot * chunkPartCoord.x, 0f, terrainChunk.Bounds.size.y / meshSettings.ChunkPartSizeRoot * chunkPartCoord.y);

                chunkParts.Add(chunkPartCoord, new ChunkPart(chunkPartCoord, partWorldPosition, terrainChunk));
            }
        }

        for (int i = 0; i < resourcePoints.Length; i++)
        {
            int x = resourcePoints[i].IndexX;
            int z = resourcePoints[i].IndexZ;
            
            float height = heightMap.Values[x + 1, z + 1];
            HeightMapLayer layer = heightMap.GetLayer(x + 1, z + 1);
            
            if (!layer.IsWater)
            {
                int chunkPartSize = uniformSize / meshSettings.ChunkPartSizeRoot + 1;

                int coordX = Mathf.FloorToInt(x / chunkPartSize) - 1;
                int coordZ = Mathf.FloorToInt(z / chunkPartSize) - 1;
                
                Vector3 pos = new Vector3(terrainChunk.Bounds.center.x, 0f, terrainChunk.Bounds.center.y) + new Vector3((x - (uniformSize - 1) / 2f) * meshSettings.MeshScale, height, (z - (uniformSize - 1) / 2f) * -meshSettings.MeshScale);

                Vector2 chunkPartCoords = terrainChunk.Coord * meshSettings.ChunkPartSizeRoot + new Vector2(coordX, -coordZ);
                ChunkPart terrainChunkPart = chunkParts[chunkPartCoords];

                terrainChunkPart.AddResourcePoint(resourcePoints[i], pos);
            }
        }
        return chunkParts;
    }
}

public struct DataMap
{
    public readonly int UniformSize;

    public readonly HeightMap HeightMap;
    public readonly BiomeMap BiomeMap;
    public readonly ResourceMap ResourceMap;
    
    public readonly Dictionary<Vector2, ChunkPart> ChunkParts;

    public DataMap(int uniformSize, HeightMap heightMap, BiomeMap biomeMap, ResourceMap resourceMap, Dictionary<Vector2, ChunkPart> chunkParts)
    {
        this.UniformSize = uniformSize;

        this.HeightMap = heightMap;
        this.BiomeMap = biomeMap;
        this.ResourceMap = resourceMap;
        this.ChunkParts = chunkParts;
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
