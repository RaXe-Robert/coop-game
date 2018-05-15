using System;
using System.Linq;
using System.Collections.Generic;

using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class WorldResourceManager : Photon.MonoBehaviour {
    private static WorldResourceManager instance;
    public static WorldResourceManager Instance
    {
        get
        {
            if (instance == null)
                instance = (new GameObject("WorldResourceManager")).AddComponent<WorldResourceManager>();
            return instance;
        }
    }

    private Dictionary<double, WorldResourceTracker> networkedWorldResources = new Dictionary<double, WorldResourceTracker>();

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this);
        else
            instance = this;
    }

    public void DecreaseHealth(WorldResource worldResource, TerrainChunk terrainChunk, float amount)
    {
        if (!networkedWorldResources.ContainsKey(worldResource.ID))
            AddNetworkedWorldResource(worldResource, terrainChunk.Coord);

        photonView.RPC(nameof(RPC_DecreaseHealth), PhotonTargets.All, worldResource.ID, terrainChunk.Coord, amount);
    }
    
    private void AddNetworkedWorldResource(WorldResource worldResource, Vector2 terrainChunkCoord)
    {
        if (worldResource == null)
            throw new MissingComponentException("No WorldResource component attached to the given prefab.");

        if (!networkedWorldResources.ContainsKey(worldResource.ID))
            photonView.RPC(nameof(RPC_AddNetworkedWorldResource), PhotonTargets.All, worldResource.ID, terrainChunkCoord, worldResource.MaxHealth);
    }

    [PunRPC]
    private void RPC_DecreaseHealth(double id, Vector2 terrainChunkCoord, float amount)
    {
        networkedWorldResources[id].Health -= amount;
        Debug.Log(networkedWorldResources[id].Health);

        if (PhotonNetwork.isMasterClient)
        {
            if (networkedWorldResources[id].Health <= 0)
            {
                photonView.RPC(nameof(RPC_RemoveNetworkedWorldResource), PhotonTargets.All, id, terrainChunkCoord);
            }
        }
    }

    [PunRPC]
    private void RPC_AddNetworkedWorldResource(double id, Vector2 terrainChunkCoord, float maxHealth)
    {
        WorldResourceTracker worldResourceTracker = new WorldResourceTracker(id, terrainChunkCoord, maxHealth);
        networkedWorldResources.Add(id, worldResourceTracker);
    }

    [PunRPC]
    private void RPC_RemoveNetworkedWorldResource(double id, Vector2 terrainChunkCoord)
    {
        Debug.Log("removing");
        networkedWorldResources.Remove(id);
        TerrainChunk terrainChunk = TerrainGenerator.GetTerrainChunk(terrainChunkCoord);

        foreach (var chunkPart in terrainChunk.DataMap.ChunkParts)
        {
            if (chunkPart.Value.ObjectPoints.ContainsKey(id))
            {
                chunkPart.Value.RemoveObjectPoint(id);
                break;
            }
        }

        TerrainGenerator.GetTerrainChunk(terrainChunkCoord)?.SaveChanges();
    }
    
    public class WorldResourceTracker
    {
        public readonly double ID;
        public readonly Vector2 TerrainChunkCoord;
        public readonly float MaxHealth;

        public float Health { get; set; }

        public WorldResourceTracker(double id, Vector2 terrainChunkCoord, float maxHealth)
        {
            this.ID = id;
            this.TerrainChunkCoord = terrainChunkCoord;
            this.MaxHealth = maxHealth;
            this.Health = maxHealth;
        }
    }
}