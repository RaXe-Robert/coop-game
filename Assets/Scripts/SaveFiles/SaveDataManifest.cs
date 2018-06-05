using System;

using UnityEngine;

[Serializable]
public class SaveDataManifest
{
    public long Save_TimeStamp;

    public long GameTime;
    public int Seed;
    public string Name;

    [SerializeField]
    public PlayerSaveInfo[] Players;

    [SerializeField]
    public ChunkSaveInfo[] Chunks;
}

[Serializable]
public class PlayerSaveInfo
{
    public string Name;
    public int Id;

    public PlayerSaveInfo(string name, int id)
    {
        this.Name = name;
        this.Id = id;
    }
}

[Serializable]
public class ChunkSaveInfo
{
    public string Name;

    public ChunkSaveInfo(string name)
    {
        this.Name = name;
    }
}