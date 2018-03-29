using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class takes a generated array of MapTiles and instantiates the propper planes and resources for the biomes.
/// 
/// The root is the gameobject that will recieve all the instantiated tiles and resources.
/// The tree and rock prefabs are the resources that will be placed on the generated tiles.
/// </summary>
public class MapDisplay : MonoBehaviour
{
    public int tileSize = 10;

    public GameObject root;
    public GameObject treePrefab;
    public GameObject rockPrefab;

    /// <summary>
    /// Places all the tiles and resources according to the generated tileMap.
    /// </summary>
    /// <param name="tileMap"></param>
    public void DrawMap(MapTile[,] tileMap, int seed)
    {
        // Clear all the gameobjects of the root
        ClearChildren(root);

        var random = new System.Random(seed);
        int width = tileMap.GetLength(0);
        int height = tileMap.GetLength(1);

        for (int y = 0; y < height; ++y)
        {
            for (int x = 0; x < width; ++x)
            {
                // Instantiates the plane and sets the name
                var plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
                plane.transform.localScale = new Vector3(10, 10, 10);
                plane.GetComponent<Renderer>().material.color = tileMap[x, y].Color;
                plane.name = $"Plane {x} {y}";

                // Calculates the tilesize and position
                tileSize = (int)plane.GetComponent<Renderer>().bounds.size.x;
                plane.transform.SetParent(root.transform);
                plane.transform.position = new Vector3(-(tileSize * width / 2) -(tileSize/2) + tileSize + (x * tileSize), 0, -(tileSize * height / 2) -(tileSize/2) + tileSize + (y * tileSize));

                // Place the resources on the newly created tile
                SpawnResourcesOnTile(plane, tileMap[x, y], random);
            }
        }
    }

    /// <summary>
    /// Spawns the resources on top of a tile
    /// </summary>
    /// <param name="tileGo"></param>
    /// <param name="mapTile"></param>
    public void SpawnResourcesOnTile(GameObject tileGo, MapTile mapTile, System.Random random)
    {
        //Variables for the biome type
        List<GameObject> resources = new List<GameObject>();
        var spawnRateMin = 5;
        var spawnRateMax = 15;

        switch (mapTile.Type)
        {
            case MapTileType.Forest:
                resources.Add(treePrefab);
                break;

            case MapTileType.RockyLand:
                resources.Add(rockPrefab);
                break;

            case MapTileType.Grassland:
                resources.Add(rockPrefab);
                resources.Add(treePrefab);
                spawnRateMax = 5;
                spawnRateMin = 0;
                break;
        }

        // If there are no resource then this method should stop
        if (resources.Count == 0)
            return;

        var amount = random.Next(spawnRateMin, spawnRateMax);
        for (int i = 0; i < amount; ++i)
        {
            //Pick a resource
            var prefab = PickRandom(resources, random);

            //Randomize the rotation, scale and location
            var rotation = new Vector3(0, random.Next(0, 360), 0);
            var scale = 1 + (float)(random.NextDouble() * 3);
            var instance = Instantiate(prefab, tileGo.transform);
            instance.transform.localRotation = Quaternion.Euler(rotation);
            instance.transform.localPosition = new Vector3(-5 + (float)random.NextDouble() * 10, 0, -5 + (float)random.NextDouble() * 10);
            instance.transform.localScale = instance.transform.localScale / tileGo.transform.lossyScale.x * scale;
        }
    }

    /// <summary>
    /// Picks a random resource from the given list
    /// </summary>
    /// <param name="items"></param>
    /// <returns></returns>
    private GameObject PickRandom(List<GameObject> items, System.Random random)
    {
        var index = random.Next(0, items.Count);
        return items[index];
    }

    /// <summary>
    /// This will clear all the child gameobjects of a gameobject
    /// </summary>
    /// <param name="root"></param>
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
