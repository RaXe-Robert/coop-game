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
        int width = heightMap.Values.GetLength(0);
        int height = heightMap.Values.GetLength(1);
        
        Color[] colorMap = new Color[width * height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
                colorMap[y * width + x] = Color.Lerp(Color.black, Color.white, Mathf.InverseLerp(heightMap.MinValue, heightMap.MaxValue, heightMap.Values[x, y]));
        }

        return TextureFromColorMap(colorMap, width, height);
    }

    public static Texture2D TextureFromBiomeMap(BiomeMap biomeMap)
    {
        int width = biomeMap.Values.GetLength(0);
        int height = biomeMap.Values.GetLength(1);
        
        Color[] colorMap = new Color[width * height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
                colorMap[y * width + x] = biomeMap.GetBiome(x, y).color;
        }

        return TextureFromColorMap(colorMap, width, height);
    }

    public static Texture2D TextureFromObjectMap(ObjectMap objectMap)
    {
        int width = objectMap.Values.GetLength(0);
        int height = objectMap.Values.GetLength(1);

        Color[] colorMap = new Color[width * height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (objectMap.Values[y,x] >= objectMap.Settings.Density)
                    colorMap[y * width + x] = Color.white;
                else
                    colorMap[y * width + x] = Color.red;
            }
        }

        return TextureFromColorMap(colorMap, width, height);
    }
}
