using Assets.Scripts.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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
    public GameObject planePrefab;
    public GameObject treePrefab;
    public GameObject rockPrefab;

    public GameObject carl;
    public GameObject carlScared;
    public GameObject fox;
    
    private NavMeshSurface navmesh;
    private List<GameObject> spawnedPlanes;

    private void Awake()
    {
        navmesh = GetComponent<NavMeshSurface>();
    }

    /// <summary>
    /// Places all the tiles and resources according to the generated tileMap.
    /// </summary>
    /// <param name="tileMap"></param>
    public void DrawMap(MapTile[,] tileMap, int seed)
    {
        // Clear all the gameobjects of the root
        ClearChildren(root);
        spawnedPlanes = new List<GameObject>();

        var random = new System.Random(seed);
        int width = tileMap.GetLength(0);
        int height = tileMap.GetLength(1);

        for (int y = 0; y < height; ++y)
        {
            for (int x = 0; x < width; ++x)
            {
                // Instantiates the plane and sets the name
                var plane = Instantiate(planePrefab);
                PhotonView view = plane.GetComponent<PhotonView>();
                view.viewID = PhotonNetwork.AllocateViewID();

                plane.transform.localScale = new Vector3(10, 10, 10);
                plane.GetComponent<Renderer>().material.color = tileMap[x, y].Color;
                plane.name = $"Plane {x} {y}";
                SetMobs(plane, tileMap[x, y].Type);

                spawnedPlanes.Add(plane);

                //If it's an Ocean tile we set the layer to be 4(Water)
                if (tileMap[x, y].Type == MapTileType.Ocean)
                    plane.layer = 4;
                else plane.layer = 8;


                // Calculates the tilesize and position
                tileSize = (int)plane.GetComponent<Renderer>().bounds.size.x;
                plane.transform.SetParent(root.transform);
                plane.transform.position = new Vector3(-(tileSize * width / 2) -(tileSize/2) + tileSize + (x * tileSize), 0, -(tileSize * height / 2) -(tileSize/2) + tileSize + (y * tileSize));

                // Place the resources on the newly created tile
                if (PhotonNetwork.isMasterClient)
                    SpawnResourcesOnTile(plane, tileMap[x, y], random);
            }
        }
        navmesh.BuildNavMesh();
    }

    public void SetMobs(GameObject plane, MapTileType type)
    {
        var mobSpawner = plane.GetComponent<MobSpawner>();
        switch(type)
        {
            case MapTileType.Forest:
                mobSpawner.mobs.Add(fox);
                break;

            case MapTileType.Desert:
                mobSpawner.mobs.Add(carl);
                break;

            case MapTileType.Grassland:
                mobSpawner.mobs.Add(carlScared);
                break;
        }
        mobSpawner.StartSpawner();
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
        var spawnRateMin = 10;
        var spawnRateMax = 30;

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
        int extent = (int)tileGo.GetComponent<Renderer>().bounds.extents.x;
        for (int i = 0; i < amount; ++i)
        {
            //Pick a resource
            var prefab = resources.PickRandom(random);

            //Randomize the rotation, scale and location
            var rotation = new Vector3(0, random.Next(0, 360), 0);
            var scale = 1 + (float)(random.NextDouble() * 3);

            Vector3 position = tileGo.transform.position + new Vector3(random.Next(-extent, extent), 0, random.Next(-extent, extent));

            GameObject resource = PhotonNetwork.InstantiateSceneObject(prefab.name, tileGo.transform.position, Quaternion.Euler(rotation), 0, null);
            GetComponent<PhotonView>().RPC("SpawnResource", PhotonTargets.AllBuffered, tileGo.name, resource.GetPhotonView().instantiationId, scale, position);
        }

        //Spawn item resources to gather before creating tools.
        switch (mapTile.Type)
        {
            case MapTileType.Forest:
                for (int i = 0; i < 5; i++)
                {
                    Vector3 position = tileGo.transform.position + new Vector3(random.Next(-extent, extent), 0.1f, random.Next(-extent, extent));
                    ItemFactory.CreateWorldObject(position, 1);
                }
                break;

            case MapTileType.RockyLand:
                for (int i = 0; i < 5; i++)
                {
                    Vector3 position = tileGo.transform.position + new Vector3(random.Next(-extent, extent), 0.1f, random.Next(-extent, extent));
                    ItemFactory.CreateWorldObject(position, 0);
                }
                break;

            case MapTileType.Grassland:
                for (int i = 0; i < 5; i++)
                {
                    Vector3 position = tileGo.transform.position + new Vector3(random.Next(-extent, extent), 0.1f, random.Next(-extent, extent));
                    ItemFactory.CreateWorldObject(position, 1);
                }
                for (int i = 0; i < 5; i++)
                {
                    Vector3 position = tileGo.transform.position + new Vector3(random.Next(-extent, extent), 0.1f, random.Next(-extent, extent));
                    ItemFactory.CreateWorldObject(position, 0);
                }
                break;
        }
    }

    [PunRPC]
    private void SpawnResource(string tileName, int resourcePhotonId, float scale, Vector3 position)
    {
        //TODO: BIG NO NO but for now it works.
        var plane = spawnedPlanes.Find(x => x.name == tileName);
        var newResource = PhotonView.Find(resourcePhotonId).gameObject;
        newResource.transform.SetParent(plane.transform);
        newResource.transform.position = position;
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
