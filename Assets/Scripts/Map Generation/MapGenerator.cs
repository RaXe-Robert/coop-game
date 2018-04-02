using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

// Type of boimes
public enum MapTileType
{
    Grassland,
    Forest,
    Desert,
    RockyLand,
    Ocean
}

/// <summary>
/// The representation of a single tile from a tile map
/// </summary>
[DebuggerDisplay("Type = {Type}")]
public class MapTile
{
    // Dictionary to map a color to a biome
    private Dictionary<MapTileType, Color> colorMap = new Dictionary<MapTileType, Color>()
    {
        { MapTileType.Grassland, new Color(0.31f, 0.81f, 0.14f) },
        { MapTileType.Forest, new Color(0.10f, 0.34f, 0.10f) },
        { MapTileType.Desert, new Color(1.00f, 0.97f, 0.76f) },
        { MapTileType.RockyLand, new Color(0.70f, 0.70f, 0.70f) },
        { MapTileType.Ocean, new Color(0.07f, 0.40f, 0.69f) }
    };

    // The parent tilemap to reference neighbours
    private MapTile[,] map;

    // Location of it self in the tilemap
    public int X { get; private set; }
    public int Y { get; private set; }

    // Boolean to check whether the tile is processed
    public bool Processed { get; set; } = false;

    // Properties of the neighbour tiles
    public MapTile North => Y > 0 ? map[X, Y - 1] : null;
    public MapTile South => Y < map.GetLength(1) - 1 ? map[X, Y + 1] : null;
    public MapTile West => X > 0 ? map[X - 1, Y] : null;
    public MapTile East => X < map.GetLength(0) - 1 ? map[X + 1, Y] : null;

    //Type of biome and the property to get the color
    public MapTileType Type { get; set; }
    public Color Color => colorMap[Type];


    /// <param name="mapTileType">The type of tile to start with, most likely to be the ocean</param>
    /// <param name="map">The tilemap where it belongs to</param>
    /// <param name="x">The x position of the tile</param>
    /// <param name="y">The y position of the tile</param>
    public MapTile(MapTileType mapTileType, ref MapTile[,] map, int x, int y)
    {
        Type = mapTileType;
        X = x;
        Y = y;
        this.map = map;
    }
}

/// <summary>
/// This is where the magic hapends, this will generate the map.
/// </summary>
public class MapGenerator : Photon.MonoBehaviour
{
    //Width and height of the map
    public int width = 24;
    public int height = 24;
    //Amount of ocean around the map
    public int borderOffset = 3;
    //The threshold for sticking to the type of the previous tile
    public float typeStickyness = .8f;
    //Whether the mapp should generate on start
    public bool generateOnStart = true;

    public int seed = 0;

    private int RandomSeed => (new System.Random()).Next(0, int.MaxValue);

    void Start()
    {
        if(PhotonNetwork.isMasterClient)
        {
            var seed = (new System.Random()).Next(0, int.MaxValue);
            photonView.RPC("SetSeedAndGenerate", PhotonTargets.AllBuffered, seed);
        }
        else if (generateOnStart)
        {
            seed = RandomSeed;
            GenerateMap();
        }
    }

    [PunRPC]
    public void SetSeedAndGenerate(int seed)
    {
        UnityEngine.Debug.Log($"Recieved generation rpc, building map with seed: {seed}");
        this.seed = seed;
        GenerateMap();
    }

    /// <summary>
    /// Generates the map and passes it to the first MapDisplay it can find
    /// </summary>
    public void GenerateMap()
    {
        // Create a MapTile array and fills it with ocean tiles
        var tiles = new MapTile[width, height];
        for (int y = 0; y < height;  y++)
            for (int x = 0; x < width; x++)
                tiles[x,y] = new MapTile(MapTileType.Ocean, ref tiles, x, y);

        // Queue to keep track of the tiles it needs to proces, the first tile will be the center one
        var queue = new Queue<Tuple<MapTile,MapTile>>();
        queue.Enqueue(new Tuple<MapTile,MapTile>(null, tiles[width / 2, height / 2]));

        // Some variables needed for the process
        System.Random random = new System.Random(seed);
        int maxWidthBorder = width - borderOffset - 1;
        int maxHeightBorder = height - borderOffset - 1;
        int iteration = 0;

        // Process the queue and generate the map
        while (queue.Count > 0)
        {
            var tuple = queue.Dequeue();
            var tile = tuple.Item2;
            var prevTile = tuple.Item1;

            // If the tile is already been processed skip it
            if (tile.Processed)
                continue;
            tile.Processed = true;

            // Adding adjecent tiles
            EnqueueAdjecentTiles(ref queue, ref tiles, tile);

            // Process the tile
            ProcessTile(random, ref tiles, ref tile, ref prevTile, maxWidthBorder, maxHeightBorder);

            iteration++;
        }

        UnityEngine.Debug.Log($"Finished generating the map in {iteration} iterations");

        MapDisplay display = FindObjectOfType<MapDisplay>();
        display.DrawMap(tiles, seed);
        
    }

    /// <summary>
    /// Adds the neighbour tiles to the queue
    /// </summary>
    /// <param name="queue"></param>
    /// <param name="tiles"></param>
    /// <param name="tile"></param>
    private void EnqueueAdjecentTiles(ref Queue<Tuple<MapTile, MapTile>> queue, ref MapTile[,] tiles, MapTile tile)
    {
        var north = tile.North;
        var east = tile.East;
        var west = tile.West;
        var south = tile.South;

        if (north != null && north.Processed == false)
            queue.Enqueue(new Tuple<MapTile, MapTile>(tile, north));
        if (west != null && west.Processed == false)
            queue.Enqueue(new Tuple<MapTile, MapTile>(tile, west));
        if (east != null && east.Processed == false)
            queue.Enqueue(new Tuple<MapTile, MapTile>(tile, east));
        if (south != null && south.Processed == false)
            queue.Enqueue(new Tuple<MapTile, MapTile>(tile, south));
    }

    /// <summary>
    /// Determine the type of the tile 
    /// </summary>
    /// <param name="random"></param>
    /// <param name="tiles"></param>
    /// <param name="mapTile"></param>
    /// <param name="prevTile"></param>
    /// <param name="maxWidthBorder"></param>
    /// <param name="maxHeightBorder"></param>
    private void ProcessTile(System.Random random, ref MapTile[,] tiles, ref MapTile mapTile, ref MapTile prevTile, int maxWidthBorder, int maxHeightBorder)
    {
        Array values = Enum.GetValues(typeof(MapTileType));
        bool stickType = (float)random.NextDouble() < typeStickyness;

        if (mapTile.X < borderOffset || mapTile.X > maxWidthBorder || mapTile.Y < borderOffset || mapTile.Y > maxHeightBorder)
            mapTile.Type = MapTileType.Ocean;
        else if (stickType && prevTile != null)
            mapTile.Type = prevTile.Type;
        else if (prevTile == null)
            mapTile.Type = MapTileType.Grassland;
        else
            mapTile.Type = (MapTileType)values.GetValue(random.Next(values.Length));
    }
}
