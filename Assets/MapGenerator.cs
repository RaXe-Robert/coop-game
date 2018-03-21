using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour {
    public int width = 256;
    public int height = 256;
    public float scale = 20f;

    private void Update()
    {
        var renderer = GetComponent<Renderer>();
        renderer.material.mainTexture = GenerateTexture();
    }

    private Texture2D GenerateTexture()
    {
        var texture = new Texture2D(width, height);

        for (int x = 0; x < 256; ++x)
        {
            for (int y = 0; y < 256; ++y)
            {
                Color c = CalculateColor(x, y);
                texture.SetPixel(x, y, c);
            }
        }

        texture.Apply();
        return texture;
    }

    private Color CalculateColor(int x, int y)
    {
        float xChoord = (float)x / width * scale;
        float yChoord = (float)y / height * scale;

        float sample = Mathf.PerlinNoise(xChoord, yChoord);
        sample = Mathf.Floor(sample / .5f) * .5f;
        return new Color(sample, sample, sample);
    }
}
