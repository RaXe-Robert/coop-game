using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDisplay : MonoBehaviour
{
    public Renderer textureRenderer;
    public int tileSize = 10;
    public System.Random random = new System.Random();

    public GameObject treePrefab;

    public void DrawMap(MapTile[,] tileMap)
    {
        var root = GameObject.Find("SpawnRoot");
        ClearChildren(root);

        int width = tileMap.GetLength(0);
        int height = tileMap.GetLength(1);

        for (int y = 0; y < height; ++y)
        {
            for (int x = 0; x < width; ++x)
            {
                var plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
                plane.transform.localScale = new Vector3(10, 10, 10);
                plane.GetComponent<Renderer>().material.color = tileMap[x, y].Color;
                plane.name = $"Plane {x} {y}";

                tileSize = (int)plane.GetComponent<Renderer>().bounds.size.x;

                plane.transform.SetParent(root.transform);
                plane.transform.position = new Vector3(tileSize + (x * tileSize), 0, tileSize + (y * tileSize));

                SpawnResourcesOnTile(plane, tileMap[x, y]);
            }
        }
    }

    public void SpawnResourcesOnTile(GameObject tileGo, MapTile mapTile)
    {
        List<GameObject> resources = new List<GameObject>();


        switch(mapTile.Type)
        {
            case MapTileType.Forest:
                var amountTrees = random.Next(5, 10);
                for (int i = 0; i < amountTrees; ++i)
                {
                    var rot = new Vector3(0, random.Next(0, 360), 0);
                    Debug.Log(rot);
                    var treeScale = 1 + (float)(random.NextDouble() * 3);
                    var tree = Instantiate(treePrefab, tileGo.transform);
                    tree.transform.localRotation = Quaternion.Euler(rot);
                    tree.transform.localPosition = new Vector3(-5 + (float)random.NextDouble() * 10, 0, -5 + (float)random.NextDouble() * 10);
                    tree.transform.localScale = tree.transform.localScale / tileGo.transform.lossyScale.x * treeScale;
                }
                break;

            case MapTileType.RockyLand:

                break;
        }
    }

    public void ClearChildren(GameObject root)
    {
        var count = root.transform.childCount - 1;
        for (int i = count; i >= 0; --i)
        {
            if (Application.isEditor)
                DestroyImmediate(root.transform.GetChild(i).gameObject);
            else
                Destroy(root.transform.GetChild(i).gameObject);
        }
    }
}
