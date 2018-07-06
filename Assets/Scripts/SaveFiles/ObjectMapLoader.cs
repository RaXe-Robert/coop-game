using System.IO;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;

using UnityEngine;

using Assets.Scripts.Map_Generation;

public static class ObjectMapLoader
{
    public static void SaveObjectMap(TerrainChunk terrainChunk, ObjectPoint[] objectPoints, string path)
    {
        string fileName = $"chunkInfo{terrainChunk.Coord.x}{terrainChunk.Coord.y}.dat";

        // Debug.Log($"Saving: {path + fileName}");

        try
        {
            using (FileStream file = File.Create(path + fileName))
            {
                BinaryFormatter bf = new BinaryFormatter();

                ObjectMapData data = new ObjectMapData
                {
                    ObjectPoints = objectPoints
                };

                bf.Serialize(file, data);
            }
        }
        catch (IOException e)
        {
            Debug.LogError(e);
        }
    }

    public static ObjectMapData LoadObjectMap(TerrainChunk terrainChunk, string path)
    {
        string fileName = $"chunkInfo{terrainChunk.Coord.x}{terrainChunk.Coord.y}.dat";

        // Debug.Log($"Loading: {path + fileName}");

        try
        {
            using (FileStream file = File.Open(path + fileName, FileMode.Open))
            {
                BinaryFormatter bf = new BinaryFormatter();

                return (ObjectMapData)bf.Deserialize(file);
            }
        }
        catch (IOException e)
        {
            Debug.LogError(e);
            return new ObjectMapData() { ObjectPoints = new ObjectPoint[0] };
        }
    }

    [System.Serializable]
    public class ObjectMapData
    {
        public ObjectPoint[] ObjectPoints;
    }
}
