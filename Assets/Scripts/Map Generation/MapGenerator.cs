using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Noise
{
    public static float[,] GenerateNoiseMap(int width, int height, float scale)
    {
        float[,] noiseMap = new float[width, height];
        scale = scale <= 0 ? 0.001f : scale;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float sampleX = x / scale;
                float sampleY = y / scale;

                float perlinValue = Mathf.PerlinNoise(sampleX, sampleY);
                noiseMap[x, y] = perlinValue;
            }
        }

        return noiseMap;
    }
}

public class MapGenerator : MonoBehaviour {
    public int width = 1024;
    public int height = 1024;
    public float scale = .5F;
    public bool autoUpdate = true;

    public void GenetateMap()
    {
        var noiseMap = Noise.GenerateNoiseMap(width, height, scale);

        MapDisplay display = FindObjectOfType<MapDisplay>();
        display.DrawNoiseMap(noiseMap);
    }


}
