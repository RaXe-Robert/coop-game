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

    private Dictionary<int, WorldResourceTracker> worldResourcesNetworked = new Dictionary<int, WorldResourceTracker>();

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this);
        else
            instance = this;
    }

    public void Update()
    {
        Vector3 playerPos = PlayerNetwork.PlayerObject?.transform.position ?? Vector3.zero;
        for (int i = 0; i < worldResourcesNetworked.Count; i++)
        {
            KeyValuePair<int, WorldResourceTracker> kvp = worldResourcesNetworked.ElementAt(i);
            if (Vector3.Distance(kvp.Value.Position, playerPos) <= 50f)
            {
                kvp.Value.SetVisible(true);
            }
            else if (kvp.Value.IsVisible)
            {
                kvp.Value.SetVisible(false);
            }
        }
    }

    public void AddNetworkedWorldResource(GameObject worldResourcePrefab, TerrainChunkController parent, TerrainInfo terrainInfo)
    {
        WorldResource worldResource = worldResourcePrefab.GetComponent<WorldResource>();
        if (worldResource == null)
            throw new MissingComponentException("No WorldResource component attached to the given prefab.");

        int id = Convert.ToInt32(terrainInfo.WorldPosition.x * 100f + terrainInfo.WorldPosition.y * 10f + terrainInfo.WorldPosition.z + (parent.TerrainChunk.SampleCenter.x + parent.TerrainChunk.SampleCenter.y) * parent.TerrainChunk.MeshSettings.MeshScale); //Point 3,2,1 gives 321 as an id

        WorldResourceTracker worldResourceTracker = new WorldResourceTracker(id, parent, worldResource, worldResourcePrefab, terrainInfo.WorldPosition);
        if (worldResourcesNetworked.ContainsKey(id) == false)
            worldResourcesNetworked.Add(id, worldResourceTracker);
        else
            Debug.LogError($"An element with the same position: `{terrainInfo.WorldPosition}` already exists.");
    }

    public class WorldResourceTracker
    {
        public readonly int ID;
        public readonly TerrainChunkController Parent;
        public readonly WorldResource WorldResource;
        public readonly GameObject WorldResourcePrefab;
        public readonly Vector3 Position;

        private WorldResource worldResourceInstance;

        private bool isVisible;
        public bool IsVisible => isVisible;
        public void SetVisible(bool state)
        {
            if (state == isVisible)
                return;

            isVisible = state;

            if (state)
            {
                if (worldResourceInstance == null)
                    worldResourceInstance = Instantiate(WorldResourcePrefab, Position, Quaternion.identity, Parent.transform).GetComponent<WorldResource>();
                else
                    Debug.LogError("Trying to create an instance but an already active instance exists!");
            }
            else
                Destroy(worldResourceInstance.gameObject);
        }

        private float health;
        public float Health => health;

        public WorldResourceTracker(int id, TerrainChunkController parent, WorldResource worldResource, GameObject worldResourcePrefab, Vector3 position)
        {
            this.ID = id;
            this.Parent = parent;
            this.WorldResource = worldResource;
            this.WorldResourcePrefab = worldResourcePrefab;
            this.Position = position;
            this.health = worldResource.MaxHealth;
        }
    }
}
