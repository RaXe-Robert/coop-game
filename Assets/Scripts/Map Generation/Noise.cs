﻿using UnityEngine;

namespace Assets.Scripts.Map_Generation
{
    public static class Noise
    {
        public enum NormalizeMode { Local, Global };

        /// <summary>
        /// Generates a Perlin Noise Map
        /// </summary>
        /// <param name="size">The width and height of the map.</param>
        /// <param name="noiseSettings">Noise Settings</param>
        /// <param name="sampleCenter">The noise offset.</param>
        /// <returns>Float[,] that contains values ranging from -1 to 1.</returns>
        public static float[,] GenerateNoiseMap(int size, NoiseSettings noiseSettings, Vector2 sampleCenter)
        {
            float[,] noiseMap = new float[size, size];

            System.Random randomNum = new System.Random(noiseSettings.seed);
            Vector2[] octaveOffsets = new Vector2[noiseSettings.octaves];

            float maxPossibleHeight = 0;
            float amplitude = 1;
            float frequency = 1;

            for (int i = 0; i < noiseSettings.octaves; i++)
            {
                float offsetX = randomNum.Next(-100000, 100000) + noiseSettings.offset.x + sampleCenter.x;
                float offsetY = randomNum.Next(-100000, 100000) - noiseSettings.offset.y - sampleCenter.y;
                octaveOffsets[i] = new Vector2(offsetX, offsetY);

                maxPossibleHeight += amplitude;
                amplitude *= noiseSettings.persistance;
            }

            float maxLocalNoiseHeight = float.MinValue;
            float minLocalNoiseHeight = float.MaxValue;

            float halfWidth = size / 2f;
            float halfHeight = size / 2f;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    amplitude = 1;
                    frequency = 1;
                    float noiseHeight = 0;

                    for (int i = 0; i < noiseSettings.octaves; i++)
                    {
                        float sampleX = (x - halfWidth + octaveOffsets[i].x) / noiseSettings.scale * frequency;
                        float sampleY = (y - halfHeight + octaveOffsets[i].y) / noiseSettings.scale * frequency;

                        float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;

                        noiseHeight += perlinValue * amplitude;

                        amplitude *= noiseSettings.persistance;
                        frequency *= noiseSettings.lacunarity;
                    }

                    if (noiseHeight > maxLocalNoiseHeight)
                        maxLocalNoiseHeight = noiseHeight;

                    if (noiseHeight < minLocalNoiseHeight)
                        minLocalNoiseHeight = noiseHeight;

                    noiseMap[x, y] = noiseHeight;

                    if (noiseSettings.normalizeMode == NormalizeMode.Global)
                    {
                        float normalizedHeight = (noiseMap[x, y] + 1) / (maxPossibleHeight / 2f);
                        noiseMap[x, y] = Mathf.Clamp(normalizedHeight, 0, int.MaxValue);
                    }
                }
            }

            if (noiseSettings.normalizeMode == NormalizeMode.Local)
            {
                for (int y = 0; y < size; y++)
                {
                    for (int x = 0; x < size; x++)
                        noiseMap[x, y] = Mathf.InverseLerp(minLocalNoiseHeight, maxLocalNoiseHeight, noiseMap[x, y]);
                }
            }
            return noiseMap;
        }
    }

    [System.Serializable]
    public class NoiseSettings
    {
        public Noise.NormalizeMode normalizeMode;

        public float scale = 50f;

        public int octaves = 6;
        [Range(0, 1)]
        public float persistance = .6f;
        public float lacunarity = 2f;

        public int seed;
        public Vector2 offset;

        public void ValidateValues()
        {
            scale = Mathf.Max(scale, 0.01f);
            octaves = Mathf.Max(octaves, 1);
            lacunarity = Mathf.Max(lacunarity, 1);
            persistance = Mathf.Clamp01(persistance);
        }
    }
}
