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
            {
                GameObject go = new GameObject("WorldResourceManager");
                instance = go.AddComponent<WorldResourceManager>();
            }
            return instance;
        }
    }

    private Dictionary<int, WorldResourceTracker> worldResources = new Dictionary<int, WorldResourceTracker>();

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
        for (int i = 0; i < worldResources.Count; i++)
        {
            KeyValuePair<int, WorldResourceTracker> kvp = worldResources.ElementAt(i);
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

    public void AddWorldResource(GameObject worldResourcePrefab, TerrainChunkController parent, TerrainInfo terrainInfo)
    {
        WorldResource worldResource = worldResourcePrefab.GetComponent<WorldResource>();
        if (worldResource == null)
            throw new MissingComponentException("No WorldResource component attached to the given prefab.");

        int id = System.Convert.ToInt32(terrainInfo.WorldPoint.x * 100f + terrainInfo.WorldPoint.y * 10f + terrainInfo.WorldPoint.z); //Point 3,2,1 gives 321 as an id

        WorldResourceTracker worldResourceTracker = new WorldResourceTracker(id, parent, worldResource, worldResourcePrefab, terrainInfo.WorldPoint);
        if (worldResources.ContainsKey(id) == false)
            worldResources.Add(id, worldResourceTracker);
        else
            Debug.LogError($"An element with the same position: `{terrainInfo.WorldPoint}` already exists.");
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
