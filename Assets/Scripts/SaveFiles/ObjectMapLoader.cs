using System.IO;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;

using UnityEngine;

using Assets.Scripts.Map_Generation;

public static class ObjectMapLoader
{
    public static void SaveObjectMap(TerrainChunk terrainChunk, ObjectPoint[] objectPoints)
    {
        string fileName = $"chunkInfo{terrainChunk.Coord.x}{terrainChunk.Coord.y}.dat";

        Debug.Log($"Saving: {TerrainGenerator.WorldDataPath + fileName}");

        try
        {
            using (FileStream file = File.Create(TerrainGenerator.WorldDataPath + fileName))
            {
                BinaryFormatter bf = new BinaryFormatter();

                MapObjectData data = new MapObjectData
                {
                    objectPoints = objectPoints
                };

                bf.Serialize(file, data);
            }
        }
        catch (IOException e)
        {
            Debug.LogError(e);
        }
    }

    public static ObjectPoint[] LoadObjectMap(TerrainChunk terrainChunk)
    {
        string fileName = $"chunkInfo{terrainChunk.Coord.x}{terrainChunk.Coord.y}.dat";

        Debug.Log($"Loading: {TerrainGenerator.WorldDataPath + fileName}");

        try
        {
            using (FileStream file = File.Open(TerrainGenerator.WorldDataPath + fileName, FileMode.Open))
            {
                BinaryFormatter bf = new BinaryFormatter();

                MapObjectData data = (MapObjectData)bf.Deserialize(file);

                return data.objectPoints;
            }
        }
        catch (IOException e)
        {
            Debug.LogError(e);
            return new ObjectPoint[0];
        }
    }

    [System.Serializable]
    public class MapObjectData
    {
        public ObjectPoint[] objectPoints;
    }
}
