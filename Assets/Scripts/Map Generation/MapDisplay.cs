using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDisplay : MonoBehaviour
{
    public Renderer textureRenderer;

    public void DrawMap(MapTile[,] tileMap)
    {
        int width = tileMap.GetLength(0);
        int height = tileMap.GetLength(1);

        Texture2D texture = new Texture2D(width, height);

        Color[] color = new Color[width * height];
        for(int y = 0; y < height; ++y)
        {
            for(int x = 0; x < width; ++x)
            {
                color[y * width + x] = tileMap[x,y].Color;
            }
        }
        texture.Apply();

        textureRenderer.sharedMaterial.mainTexture = texture;
        textureRenderer.transform.localScale = new Vector3(width, 1, height);
    }
}
