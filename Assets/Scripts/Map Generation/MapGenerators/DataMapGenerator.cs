using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
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

        Dictionary<Vector2, ChunkPart> chunkParts = CreateChunkParts(uniformSize, meshSettings, heightMap, resourceMap, terrainChunk);

        return new DataMap(uniformSize, heightMap, biomeMap, resourceMap, chunkParts);
    }

    private static Dictionary<Vector2, ChunkPart> CreateChunkParts(int uniformSize, MeshSettings meshSettings, HeightMap heightMap, ResourceMap resourceMap, TerrainChunk terrainChunk)
    {
        Dictionary<Vector2, ChunkPart> chunkParts = new Dictionary<Vector2, ChunkPart>();

        ObjectPoint[] objectPoints = resourceMap.ObjectPoints;

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

        FillChunkParts(uniformSize, meshSettings, heightMap, terrainChunk, chunkParts, objectPoints);

        return chunkParts;
    }

    private static void FillChunkParts(int uniformSize, MeshSettings meshSettings, HeightMap heightMap, TerrainChunk terrainChunk, Dictionary<Vector2, ChunkPart> chunkParts, ObjectPoint[] objectPoints)
    {
        string fileName = $"/chunkInfo{terrainChunk.Coord.x}{terrainChunk.Coord.y}.dat";
        
        if (File.Exists(TerrainGenerator.WorldDataPath + fileName))
            objectPoints = Load(terrainChunk);
        else
            Save(terrainChunk, objectPoints);

        // Fill chunks
        for (int i = 0; i < objectPoints.Length; i++)
        {
            int x = objectPoints[i].IndexX;
            int z = objectPoints[i].IndexZ;

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

                terrainChunkPart.AddObjectPoint(objectPoints[i], pos);
            }
        }
    }

    public static void Save(TerrainChunk terrainChunk, ObjectPoint[] objectPoints)
    {
        string fileName = $"/chunkInfo{terrainChunk.Coord.x}{terrainChunk.Coord.y}.dat";

        try
        {
            Debug.Log($"Saving: {TerrainGenerator.WorldDataPath + fileName}");

            if (!Directory.Exists(TerrainGenerator.WorldDataPath))
                Directory.CreateDirectory(TerrainGenerator.WorldDataPath);

            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(TerrainGenerator.WorldDataPath + fileName);

            MapObjectData data = new MapObjectData
            {
                objectPoints = objectPoints
            };

            bf.Serialize(file, data);
            file.Close();
        }
        catch (IOException e)
        {
            Debug.LogError(e);
        }
    }

    public static ObjectPoint[] Load(TerrainChunk terrainChunk)
    {
        string fileName = $"/chunkInfo{terrainChunk.Coord.x}{terrainChunk.Coord.y}.dat";

        try
        {
            Debug.Log($"Loading: {TerrainGenerator.WorldDataPath + fileName}");

            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(TerrainGenerator.WorldDataPath + fileName, FileMode.Open);
            MapObjectData data = (MapObjectData)bf.Deserialize(file);
            file.Close();

            return data.objectPoints;
        }
        catch (IOException e)
        {
            Debug.LogError(e);
            return new ObjectPoint[0];
        }
    }

    [System.Serializable]
    public class MapObjectData
    {
        public ObjectPoint[] objectPoints;
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
