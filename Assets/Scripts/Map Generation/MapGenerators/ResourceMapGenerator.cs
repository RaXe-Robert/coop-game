using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Map_Generation
{
    public class ResourceMapGenerator
    {
        public static ResourceMap GenerateResourceMap(int size, ResourceMapSettings settings, Vector2 sampleCenter, BiomeMap biomeMap, HeightMap heightMap)
        {
            size -= 1; // To avoid overlap with other chunks on edges

            float[,] values = new float[size,size];
            
            // Create a unique seed for this chunk
            System.Random randomNum = new System.Random((int)(0.5 * (sampleCenter.x + sampleCenter.y) * (sampleCenter.x + sampleCenter.y + 1) + sampleCenter.y + settings.NoiseSettings.seed));

            List<ResourcePoint> resourcePoints = new List<ResourcePoint>();

            for (int x = 0; x < size; x++)
            {
                for (int z = 0; z < size; z++)
                {
                    float random = randomNum.Next(0, 100) / 100f;

                    Biome biome = biomeMap.GetBiome(x, z);

                    if (random > biome.ResourceDensity)
                    {
                        int biomeId = (int)biome.BiomeType;

                        if (TerrainGenerator.BiomeResources.ContainsKey(biomeId) && TerrainGenerator.BiomeResources[biomeId].worldResourceEntries.Count > 0)
                        {
                            int resourceIndex = randomNum.Next(0, TerrainGenerator.BiomeResources[biomeId].worldResourceEntries.Count);

                            resourcePoints.Add(new ResourcePoint(x, z, biomeId, resourceIndex));
                        }
                    }
                }
            }

            return new ResourceMap(values, resourcePoints.ToArray(), settings);
        }
    }

    public class BiomeResources
    {
        public readonly string biomeName;
        public readonly int biomeIndex;

        public List<WorldResourceEntry> worldResourceEntries;

        public BiomeResources(string biomeName, int biomeIndex)
        {
            this.biomeName = biomeName;
            this.biomeIndex = biomeIndex;

            worldResourceEntries = new List<WorldResourceEntry>();
        }
    }

    public struct ResourceMap
    {
        public readonly float[,] values;

        public readonly ResourcePoint[] resourcePoints;

        public readonly ResourceMapSettings settings;

        public ResourceMap(float[,] values, ResourcePoint[] resourcePoints, ResourceMapSettings settings)
        {
            this.values = values;
            this.resourcePoints = resourcePoints;
            this.settings = settings;
        }
    }

    public struct ResourcePoint
    {
        public readonly int x;
        public readonly int z;
        public readonly int biomeId;
        public readonly int worldResourcePrefabId;

        public ResourcePoint(int x, int z, int biomeId, int worldResourcePrefabId)
        {
            this.x = x;
            this.z = z;
            this.biomeId = biomeId;
            this.worldResourcePrefabId = worldResourcePrefabId;
        }
    }
}


