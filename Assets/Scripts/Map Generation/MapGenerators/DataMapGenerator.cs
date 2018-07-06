﻿using System;
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
            FillChunkParts(uniformSize, ref chunkParts, meshSettings, resourceMapSettings, biomeMap, heightMap, terrainChunk);

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

        private static void FillChunkParts(int uniformSize, ref Dictionary<Vector2, TerrainChunkPart> chunkParts, MeshSettings meshSettings, ResourceMapSettings resourceMapSettings, BiomeMap biomeMap, HeightMap heightMap, TerrainChunk terrainChunk)
        {
            ObjectPoint[] objectPoints = CreateObjectPoints(uniformSize, meshSettings, resourceMapSettings, biomeMap, heightMap, terrainChunk);

            // Fill chunk parts
            for (int i = 0; i < objectPoints.Length; i++)
            {
                Vector2 chunkPartCoords = new Vector2(objectPoints[i].chunkPartCoordX, objectPoints[i].chunkPartCoordZ);
                TerrainChunkPart terrainChunkPart = chunkParts[chunkPartCoords];

                terrainChunkPart.AddObjectPoint(objectPoints[i]);
            }
        }

        public static ObjectPoint[] CreateObjectPoints(int uniformSize, MeshSettings meshSettings, ResourceMapSettings resourceMapSettings, BiomeMap biomeMap, HeightMap heightMap, TerrainChunk terrainChunk)
        {
            string fileName = $"chunkInfo{terrainChunk.Coord.x}{terrainChunk.Coord.y}.dat";
            
            // Try to load a save file
            if (File.Exists(SaveDataManager.WorldDataPath + fileName))
                return ObjectMapLoader.LoadObjectMap(terrainChunk, SaveDataManager.WorldDataPath).ObjectPoints;
            else
            {
                //ResourceMap resourceMap = ResourceMapGenerator.GenerateResourceMap(uniformSize, resourceMapSettings, terrainChunk.SampleCenter);

                ResourceMap resourceMap = ResourceMapGenerator.GenerateResourceMap(uniformSize, resourceMapSettings, terrainChunk.SampleCenter, biomeMap, heightMap);

                List<ObjectPoint> tempObjectPoints = new List<ObjectPoint>();

                // Resource points to object points
                foreach (var resourcePoint in resourceMap.resourcePoints)
                {
                    int x = resourcePoint.x;
                    int z = resourcePoint.z;

                    HeightMapLayer layer = heightMap.GetLayer(x + 1, z + 1);

                    // Don't spawn objects on water.
                    if (layer.IsWater)
                        continue;

                    float height = heightMap.Values[x + 1, z + 1];
                    Vector3 position = new Vector3(terrainChunk.Bounds.center.x, 0f, terrainChunk.Bounds.center.y) + new Vector3((x - (uniformSize - 1) / 2f) * meshSettings.MeshScale, height, (z - (uniformSize - 1) / 2f) * -meshSettings.MeshScale);

                    // Create a seeded System.Random for the rotation based on the position
                    float a = position.x + position.y;
                    float b = position.z + position.y;
                    System.Random rand = new System.Random((int)(0.5 * (a + b) * (a + b + 1) + b));

                    Quaternion rotation = Quaternion.Euler(new Vector3(0f, rand.Next(0, 360), 0f));

                    int chunkPartSize = uniformSize / meshSettings.ChunkPartSizeRoot + 1;
                    int coordX = Mathf.FloorToInt(x / chunkPartSize) - 1;
                    int coordZ = Mathf.FloorToInt(z / chunkPartSize) - 1;

                    Vector2 chunkPartCoords = terrainChunk.Coord * meshSettings.ChunkPartSizeRoot + new Vector2(coordX, -coordZ);

                    tempObjectPoints.Add(new ObjectPoint(position, rotation, resourcePoint.biomeId, resourcePoint.worldResourcePrefabId, chunkPartCoords.x, chunkPartCoords.y));
                }

                return tempObjectPoints.ToArray();
            }
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
        public readonly int biomeId;
        public readonly int worldResourcePrefabId;

        public readonly float chunkPartCoordX;
        public readonly float chunkPartCoordZ;

        public ObjectPoint(Vector3 position, Quaternion rotation, int biomeId, int worldResourcePrefabId, float chunkPartCoordX, float chunkPartCoordZ)
        {
            this.position = position;
            this.rotation = rotation;
            this.biomeId = biomeId;
            this.worldResourcePrefabId = worldResourcePrefabId;

            this.chunkPartCoordX = chunkPartCoordX;
            this.chunkPartCoordZ = chunkPartCoordZ;
        }
    }
}
