using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class PlayerDataLoader
{
    public static void SavePlayerData(PhotonPlayer player, SerializableVector3 position, string path)
    {
        string fileName = $"player{(int)player.CustomProperties["UniqueID"]}.dat";

        Debug.Log($"Saving: {path + fileName}");

        try
        {
            using (FileStream file = File.Create(path + fileName))
            {
                BinaryFormatter bf = new BinaryFormatter();

                PlayerData data = new PlayerData
                {
                    Id = (int)player.CustomProperties["UniqueID"],
                    Position = position
                };

                bf.Serialize(file, data);
            }
        }
        catch (IOException e)
        {
            Debug.LogError(e);
        }
    }

    public static PlayerData LoadPlayerData(PhotonPlayer player, string path)
    {
        string fileName = $"player{player.CustomProperties["UniqueID"]}.dat";

        Debug.Log($"Loading: {path + fileName}");

        try
        {
            using (FileStream file = File.Open(path + fileName, FileMode.Open))
            {
                BinaryFormatter bf = new BinaryFormatter();

                return (PlayerData)bf.Deserialize(file);
            }
        }
        catch (IOException e)
        {
            Debug.LogError(e);
            return new PlayerData()
            {
                Id = (int)player.CustomProperties["UniqueID"],
                Position = Vector3.zero
            };
        }
    }

    [System.Serializable]
    public class PlayerData
    {
        public int Id;
        public SerializableVector3 Position;
    }
}
