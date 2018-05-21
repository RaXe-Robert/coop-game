using System;

using UnityEngine;

[Serializable]
public class SaveDataManifest
{
    public long TimeStamp;

    [SerializeField]
    public PlayerSaveInfo[] Players;

    [SerializeField]
    public ChunkSaveInfo[] Chunks;
}

[Serializable]
public class PlayerSaveInfo
{
    public string Name;

    public PlayerSaveInfo(string name)
    {
        this.Name = name;
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