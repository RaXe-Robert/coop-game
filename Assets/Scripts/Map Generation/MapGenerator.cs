using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public enum MapTileType
{
    Grassland,
    Forest,
    Desert,
    RockyLand,
    Ocean
}

[DebuggerDisplay("Type = {Type}")]
public class MapTile
{
    private Dictionary<MapTileType, Color> colorMap = new Dictionary<MapTileType, Color>()
    {
        { MapTileType.Grassland, Color.green },
        { MapTileType.Forest, Color.cyan },
        { MapTileType.Desert, Color.red },
        { MapTileType.RockyLand, Color.gray },
        { MapTileType.Ocean, Color.blue }
    };

    private MapTile[,] map;

    public int X { get; private set; }
    public int Y { get; private set; }
    public bool Processed { get; set; } = false;

    public MapTile North => Y > 0 ? map[X, Y - 1] : null;
    public MapTile South => Y < map.GetLength(1) - 1 ? map[X, Y + 1] : null;
    public MapTile West => X > 0 ? map[X - 1, Y] : null;
    public MapTile East => X < map.GetLength(0) - 1 ? map[X + 1, Y] : null;

    public MapTileType Type { get; set; }
    public Color Color => colorMap[Type];

    public MapTile(MapTileType mapTileType, ref MapTile[,] map, int x, int y)
    {
        Type = mapTileType;
        X = x;
        Y = y;
        this.map = map;
    }
}

public class MapGenerator : MonoBehaviour
{
    public int width = 256;
    public int height = 256;
    public int borderOffset = 3;
    public float typeStickyness = .8f;

    public void GenetateMap()
    {
        var tiles = new MapTile[width, height];

        for (int y = 0; y < height;  y++)
            for (int x = 0; x < width; x++)
                tiles[x,y] = new MapTile(MapTileType.Ocean, ref tiles, x, y);

        var queue = new Queue<Tuple<MapTile,MapTile>>();
        queue.Enqueue(new Tuple<MapTile,MapTile>(null, tiles[width / 2, height / 2]));

        System.Random random = new System.Random((int)DateTime.Now.Ticks);
        int maxWidthBorder = width - borderOffset - 1;
        int maxHeightBorder = height - borderOffset - 1;
        while (queue.Count > 0)
        {
            var tuple = queue.Dequeue();
            var tile = tuple.Item2;
            var prevTile = tuple.Item1;
            tile.Processed = true;

            //Adding adjecent tiles
            EnqueueAdjecentTiles(ref queue, ref tiles, tile);

            ProcessTile(random, ref tiles, ref tile, ref prevTile, maxWidthBorder, maxHeightBorder);
        }
        MapDisplay display = FindObjectOfType<MapDisplay>();
        display.DrawMap(tiles);
    }

    private void EnqueueAdjecentTiles(ref Queue<Tuple<MapTile, MapTile>> queue, ref MapTile[,] tiles, MapTile tile)
    {
        var north = tile.North;
        var east = tile.East;
        var west = tile.West;
        var south = tile.South;

        if (north?.Processed == false)
            queue.Enqueue(new Tuple<MapTile, MapTile>(tile, north));
        if (west?.Processed == false)
            queue.Enqueue(new Tuple<MapTile, MapTile>(tile, west));
        if (east?.Processed == false)
            queue.Enqueue(new Tuple<MapTile, MapTile>(tile, east));
        if (south?.Processed == false)
            queue.Enqueue(new Tuple<MapTile, MapTile>(tile, south));
    }

    private void ProcessTile(System.Random random, ref MapTile[,] tiles, ref MapTile mapTile, ref MapTile prevTile, int maxWidthBorder, int maxHeightBorder)
    {
        Array values = Enum.GetValues(typeof(MapTileType));
        bool stickType = (float)random.NextDouble() < typeStickyness;

        if (mapTile.X < borderOffset || mapTile.X > maxWidthBorder || mapTile.Y < borderOffset || mapTile.Y > maxHeightBorder)
            mapTile.Type = MapTileType.Ocean;
        else if (stickType && prevTile != null)
            mapTile.Type = prevTile.Type;
        else
            mapTile.Type = (MapTileType)values.GetValue(random.Next(values.Length));
    }
}
