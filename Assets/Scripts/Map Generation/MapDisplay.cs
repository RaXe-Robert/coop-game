using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDisplay : MonoBehaviour
{
    public Renderer textureRenderer;
    public int tileSize = 10;

    public void DrawMap(MapTile[,] tileMap)
    {
        var root = GameObject.Find("SpawnRoot");
        ClearChildren(root);

        int width = tileMap.GetLength(0);
        int height = tileMap.GetLength(1);

        for(int y = 0; y < height; ++y)
        {
            for(int x = 0; x < width; ++x)
            {
                var plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
                plane.GetComponent<Renderer>().material.color = tileMap[x, y].Color;
                plane.name = $"Plane {x} {y}";

                tileSize = (int)plane.GetComponent<Renderer>().bounds.size.x;

                plane.transform.SetParent(root.transform);
                plane.transform.position = new Vector3(tileSize + (x * tileSize), 0, tileSize + (y * tileSize));
            }
        }
    }

    public void ClearChildren(GameObject root)
    {
        var count = root.transform.childCount-1;
        for (int i = count; i >= 0; --i)
        {
            if (Application.isEditor)
                DestroyImmediate(root.transform.GetChild(i).gameObject);
            else
                Destroy(root.transform.GetChild(i).gameObject);
        }
    }
}
