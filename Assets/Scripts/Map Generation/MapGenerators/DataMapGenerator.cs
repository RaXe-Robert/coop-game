using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Assets.Scripts.Map_Generation
{
    public class DataMapGenerator
    {
        /// <summary>
        /// Generates a <see cref="DataMap"/> that consists of a heightmap, biomeMap and resourceMap based on a single size. Divides the resourceMap in <see cref="DataMap.ChunkParts"/> based on <see cref="MeshSettings.ChunkPartSizeRoot"/>
        /// </summary>
        /// <param name="terrainChunk">The terrain chunk that requested the DataMap.</param>
        /// <returns></returns>
        public static DataMap GenerateDataMap(MeshSettings meshSettings, HeightMapSettings heightMapSettings, BiomeMapSettings biomeMapSettings, ResourceMapSettings resourceMapSettings, TerrainChunk terrainChunk)
        {
            int size = meshSettings.NumVertsPerLine;
            int uniformSize = size - 2;

            // Generate data maps
            HeightMap heightMap = HeightMapGenerator.GenerateHeightMap(size, heightMapSettings, terrainChunk.SampleCenter);
            BiomeMap biomeMap = BiomeMapGenerator.GenerateBiomeMap(uniformSize, biomeMapSettings, terrainChunk.SampleCenter);

            // Create chunk parts 
            Dictionary<Vector2, TerrainChunkPart> chunkParts = CreateChunkParts(uniformSize, meshSettings, terrainChunk);
            FillChunkParts(uniformSize, ref chunkParts, meshSettings, resourceMapSettings, heightMap, terrainChunk);

            return new DataMap(uniformSize, heightMap, biomeMap, chunkParts);
        }

        private static Dictionary<Vector2, TerrainChunkPart> CreateChunkParts(int uniformSize, MeshSettings meshSettings, TerrainChunk terrainChunk)
        {
            Dictionary<Vector2, TerrainChunkPart> chunkParts = new Dictionary<Vector2, TerrainChunkPart>();

            int chunkRangeHalf = Mathf.FloorToInt(meshSettings.ChunkPartSizeRoot / 2f); // ONLY WORKS FOR UNEVEN NUMBERS AT THE MOMENT

            for (int x = -chunkRangeHalf; x <= chunkRangeHalf; x++)
            {
                for (int z = -chunkRangeHalf; z <= chunkRangeHalf; z++)
                {
                    Vector2 chunkPartCoord = terrainChunk.Coord * meshSettings.ChunkPartSizeRoot + new Vector2(x, z);
                    Vector3 partWorldPosition = new Vector3(terrainChunk.Bounds.size.x / meshSettings.ChunkPartSizeRoot * chunkPartCoord.x, 0f, terrainChunk.Bounds.size.y / meshSettings.ChunkPartSizeRoot * chunkPartCoord.y);

                    chunkParts.Add(chunkPartCoord, new TerrainChunkPart(chunkPartCoord, partWorldPosition, terrainChunk));
                }
            }

            return chunkParts;
        }

        private static void FillChunkParts(int uniformSize, ref Dictionary<Vector2, TerrainChunkPart> chunkParts, MeshSettings meshSettings, ResourceMapSettings resourceMapSettings, HeightMap heightMap, TerrainChunk terrainChunk)
        {
            string fileName = $"chunkInfo{terrainChunk.Coord.x}{terrainChunk.Coord.y}.dat";

            ObjectPoint[] objectPoints;

            if (File.Exists(TerrainGenerator.WorldDataPath + fileName))
                objectPoints = LoadObjectMap(terrainChunk);
            else
            {
                ResourceMap resourceMap = ResourceMapGenerator.GenerateResourceMap(uniformSize, resourceMapSettings, terrainChunk.SampleCenter);

                List<ObjectPoint> tempObjectPoints = new List<ObjectPoint>();

                // Resource points to object points
                foreach (var resourcePoint in resourceMap.ResourcePoints)
                {
                    int x = resourcePoint.x;
                    int z = resourcePoint.z;

                    HeightMapLayer layer = heightMap.GetLayer(x + 1, z + 1);

                    if (layer.IsWater)
                        continue;

                    float height = heightMap.Values[x + 1, z + 1];
                    Vector3 position = new Vector3(terrainChunk.Bounds.center.x, 0f, terrainChunk.Bounds.center.y) + new Vector3((x - (uniformSize - 1) / 2f) * meshSettings.MeshScale, height, (z - (uniformSize - 1) / 2f) * -meshSettings.MeshScale);
                    Quaternion rotation = Quaternion.identity;

                    int chunkPartSize = uniformSize / meshSettings.ChunkPartSizeRoot + 1;

                    int coordX = Mathf.FloorToInt(x / chunkPartSize) - 1;
                    int coordZ = Mathf.FloorToInt(z / chunkPartSize) - 1;

                    Vector2 chunkPartCoords = terrainChunk.Coord * meshSettings.ChunkPartSizeRoot + new Vector2(coordX, -coordZ);

                    tempObjectPoints.Add(new ObjectPoint(position, rotation, resourcePoint.WorldResourcePrefabID, chunkPartCoords.x, chunkPartCoords.y));
                }

                objectPoints = tempObjectPoints.ToArray();
            }

            // Fill chunk parts
            for (int i = 0; i < objectPoints.Length; i++)
            {
                Vector2 chunkPartCoords = new Vector2(objectPoints[i].chunkPartCoordX, objectPoints[i].chunkPartCoordZ);
                TerrainChunkPart terrainChunkPart = chunkParts[chunkPartCoords];

                terrainChunkPart.AddObjectPoint(objectPoints[i]);
            }
        }

        public static void SaveObjectMap(TerrainChunk terrainChunk, ObjectPoint[] objectPoints)
        {
            string fileName = $"chunkInfo{terrainChunk.Coord.x}{terrainChunk.Coord.y}.dat";

            Debug.Log($"Saving: {TerrainGenerator.WorldDataPath + fileName}");

            FileStream file = File.Create(TerrainGenerator.WorldDataPath + fileName);
            try
            {
                BinaryFormatter bf = new BinaryFormatter();

                MapObjectData data = new MapObjectData
                {
                    objectPoints = objectPoints
                };

                bf.Serialize(file, data);
            }
            catch (IOException e)
            {
                Debug.LogError(e);
            }
            finally
            {
                file.Close();

            }
        }

        public static ObjectPoint[] LoadObjectMap(TerrainChunk terrainChunk)
        {
            string fileName = $"chunkInfo{terrainChunk.Coord.x}{terrainChunk.Coord.y}.dat";

            FileStream file = File.Open(TerrainGenerator.WorldDataPath + fileName, FileMode.Open);
            try
            {
                Debug.Log($"Loading: {TerrainGenerator.WorldDataPath + fileName}");

                BinaryFormatter bf = new BinaryFormatter();
                MapObjectData data = (MapObjectData)bf.Deserialize(file);

                return data.objectPoints;
            }
            catch (IOException e)
            {
                Debug.LogError(e);
                return new ObjectPoint[0];
            }
            finally
            {
                file.Close();
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

        public readonly Dictionary<Vector2, TerrainChunkPart> ChunkParts;

        public DataMap(int uniformSize, HeightMap heightMap, BiomeMap biomeMap, Dictionary<Vector2, TerrainChunkPart> chunkParts)
        {
            this.UniformSize = uniformSize;

            this.HeightMap = heightMap;
            this.BiomeMap = biomeMap;
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
            return BiomeMap.GetBiome(x, z);
        }
    }

    [System.Serializable]
    public struct ObjectPoint
    {
        public readonly SerializableVector3 position;
        public readonly SerializableQuaternion rotation;
        public readonly int WorldResourcePrefabID;

        public readonly float chunkPartCoordX;
        public readonly float chunkPartCoordZ;

        public ObjectPoint(Vector3 position, Quaternion rotation, int worldResourcePrefabID, float chunkPartCoordX, float chunkPartCoordZ)
        {
            this.position = position;
            this.rotation = rotation;
            this.WorldResourcePrefabID = worldResourcePrefabID;

            this.chunkPartCoordX = chunkPartCoordX;
            this.chunkPartCoordZ = chunkPartCoordZ;
        }
    }
}
