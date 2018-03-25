using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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

    public MapTileType Type { get; private set; }
    public Color Color {
        get
        {
            return colorMap[Type];
        }
    }

    public MapTile(MapTileType mapTileType)
    {
        Type = mapTileType;
    }
}

public class MapGenerator : MonoBehaviour
{
    public int width = 256;
    public int height = 256;
    public int borderOffset = 3;

    public MapTile[,] GenetateMap()
    {
        var offsetMaxWidth = width - borderOffset;
        var offsetMaxHeight = height - borderOffset;
        var tiles = new MapTile[width, height];

        for (int y = 0; y < height;  y++)
        {
            for (int x = 0; x < width; x++)
            {
                MapTile tile;

                if (x < borderOffset || y < borderOffset || x > offsetMaxWidth || y > offsetMaxHeight)
                    tile = new MapTile(MapTileType.Ocean);
                else
                    tile = new MapTile(MapTileType.Grassland);

                tiles[x, y] = tile;
            }
        }

        MapDisplay display = FindObjectOfType<MapDisplay>();
        display.DrawMap(tiles);
        return tiles;
    }
}
