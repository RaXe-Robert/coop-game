﻿using UnityEngine;
using System.Collections;

namespace Assets.Scripts.Map_Generation
{
    public static class HeightMapGenerator
    {
        public static HeightMap GenerateHeightMap(int size, HeightMapSettings settings, Vector2 sampleCenter)
        {
            float[,] values = Noise.GenerateNoiseMap(size, settings.NoiseSettings, sampleCenter);

            AnimationCurve heightCurve_threadSafe = new AnimationCurve(settings.HeightCurve.keys);

            float minValue = float.MaxValue;
            float maxValue = float.MinValue;

            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    values[x, y] *= heightCurve_threadSafe.Evaluate(values[x, y]) * settings.HeightMultiplier;



                    if (values[x, y] > maxValue)
                        maxValue = values[x, y];
                    if (values[x, y] < minValue)
                        minValue = values[x, y];
                }
            }
            return new HeightMap(values, minValue, maxValue, settings);
        }
    }

    public struct HeightMap
    {
        public readonly float[,] Values;
        public readonly float MinValue;
        public readonly float MaxValue;

        public readonly HeightMapSettings Settings;

        public HeightMap(float[,] values, float minValue, float maxValue, HeightMapSettings settings)
        {
            this.Values = values;
            this.MinValue = minValue;
            this.MaxValue = maxValue;
            this.Settings = settings;
        }

        public HeightMapLayer GetLayer(int x, int z)
        {
            float heightPercent = Mathf.Clamp01((Values[x, z] - MinValue) / (MaxValue - MinValue));

            for (int i = Settings.Layers.Length - 1; i >= 0; i--)
            {
                if (heightPercent >= Settings.Layers[i].StartHeight)
                    return Settings.Layers[i];
            }

            return null;
        }
    }
}
