using UnityEngine;
using System.Collections;

public static class TextureGenerator
{
    public static Texture2D TextureFromColorMap(Color[] colorMap, int width, int height)
    {
        Texture2D texture = new Texture2D(width, height)
        {
            filterMode = FilterMode.Point,
            wrapMode = TextureWrapMode.Clamp
        };
        texture.SetPixels(colorMap);
        texture.Apply();

        return texture;
    }

    public static Texture2D TextureFromHeightMap(HeightMap heightMap)
    {
        int width = heightMap.values.GetLength(0);
        int height = heightMap.values.GetLength(1);
        
        Color[] colorMap = new Color[width * height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
                colorMap[y * width + x] = Color.Lerp(Color.black, Color.white, Mathf.InverseLerp(heightMap.minValue, heightMap.maxValue, heightMap.values[x, y]));
        }

        return TextureFromColorMap(colorMap, width, height);
    }

    public static Texture2D TextureFromBiomeMap(BiomeMap biomeMap)
    {
        int width = biomeMap.values.GetLength(0);
        int height = biomeMap.values.GetLength(1);
        int numOfBiomes = biomeMap.numOfBiomes;
        
        float range = biomeMap.maxValue - biomeMap.minValue;

        Color[] colorMap = new Color[width * height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // Set the color of the first biome
                colorMap[y * width + x] = biomeMap.settings.Biomes[0].color;

                if (numOfBiomes > 2)
                {
                    // Loop through all other biomes
                    for (int biomeIndex = 1; biomeIndex < numOfBiomes; biomeIndex++)
                    {
                        if (biomeMap.values[x, y] > range / numOfBiomes * biomeIndex)
                        {
                            colorMap[y * width + x] = biomeMap.settings.Biomes[biomeIndex].color;
                        }
                    }
                }
            }
        }

        return TextureFromColorMap(colorMap, width, height);
    }
}
