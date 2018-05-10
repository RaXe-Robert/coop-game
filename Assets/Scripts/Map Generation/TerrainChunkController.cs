using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controller for a specific <see cref="global::TerrainChunk"/> instance. Manages the resources loading based on the data within the TerrainChunk.
/// </summary>
public class TerrainChunkController : MonoBehaviour
{
    private const float chunkPartDistanceThreshold = 50f;
    private const float sqrChunkPartDistanceThreshold = chunkPartDistanceThreshold * chunkPartDistanceThreshold;

    private const int numOfChunkPartsRoot = 3;
    private const int numOfChunkParts = numOfChunkPartsRoot * numOfChunkPartsRoot;

    public TerrainChunk TerrainChunk { get; private set; }
    public bool IsVisible => TerrainChunk.IsVisible;
    public bool IsPrimaryLODActive => TerrainChunk.CurrentLODIndex == 0; 

    public bool Loaded { get; private set; } = false;

    private readonly Dictionary<Vector2, ChunkPart> chunkParts = new Dictionary<Vector2, ChunkPart>(); 

    private Vector2 viewerPositionOld;
    
    public ChunkPart GetChunkPartByIndex(int x, int z)
    {
        if (x < 0 || z < 0 || x >= TerrainChunk.DataMap.UniformSize || z >= TerrainChunk.DataMap.UniformSize)
            throw new System.IndexOutOfRangeException("Index was greater than the DataMap size.");

        int chunkPartSize = TerrainChunk.DataMap.UniformSize / numOfChunkPartsRoot + 1;
        
        int coordX = Mathf.FloorToInt(x / chunkPartSize) - 1;
        int coordZ = Mathf.FloorToInt(z / chunkPartSize) - 1;

        Vector2 chunkPartCoords = TerrainChunk.Coord * numOfChunkPartsRoot + new Vector2(coordX, -coordZ);

        return chunkParts[chunkPartCoords];
    }

    private void Update()
    {
        if (!Loaded)
            return;

        if (TerrainChunk.IsVisible && IsPrimaryLODActive)
        {
            Vector2 viewerPosition = TerrainChunk.ViewerPosition;


            if ((viewerPositionOld - viewerPosition).sqrMagnitude > 5f)
            {
                viewerPositionOld = viewerPosition;

                int currentPartCoordX = Mathf.RoundToInt(viewerPosition.x / TerrainChunk.MeshSettings.MeshWorldSize * numOfChunkPartsRoot);
                int currentPartCoordZ = Mathf.RoundToInt(viewerPosition.y / TerrainChunk.MeshSettings.MeshWorldSize * numOfChunkPartsRoot);

                HashSet<Vector2> visibleChunkPartCoords = new HashSet<Vector2>();
                int chunkPartViewDistance = 1;

                // Loop through all chunk part coords that are in range of the players coord and set them visible
                for (int x = -chunkPartViewDistance; x <= chunkPartViewDistance; x++)
                {
                    for (int z = -chunkPartViewDistance; z <= chunkPartViewDistance; z++)
                    {
                        Vector2 viewedChunkPartCoord = new Vector2(x + currentPartCoordX, z + currentPartCoordZ);
                        if (chunkParts.ContainsKey(viewedChunkPartCoord))
                        {
                            chunkParts[viewedChunkPartCoord].IsVisible = true;
                            visibleChunkPartCoords.Add(viewedChunkPartCoord);
                        }

                    }
                }

                // Set all the reamining chunk parts invisible 
                foreach (var kvp in chunkParts)
                {
                    if (visibleChunkPartCoords.Contains(kvp.Key))
                        continue;
                    else
                        kvp.Value.IsVisible = false;
                }
            }
        }
    }

    public void Load(TerrainChunk terrainChunk)
    {
        TerrainChunk = terrainChunk;
        Loaded = true;

        Vector2 chunkCoord = terrainChunk.Coord * numOfChunkPartsRoot; // Multiply by total parts root

        for (int x = -1; x <= 1; x++)
        {
            for (int z = -1; z <= 1; z++)
            {
                Vector2 chunkPartCoord = chunkCoord + new Vector2(x, z);
                Vector3 partWorldPosition = new Vector3(TerrainChunk.Bounds.size.x / numOfChunkPartsRoot * chunkPartCoord.x, 0f, TerrainChunk.Bounds.size.y / numOfChunkPartsRoot * chunkPartCoord.y);

                chunkParts.Add(chunkPartCoord, new ChunkPart(chunkPartCoord, partWorldPosition, this));
            }
        }

        LoadResources();
    }

    /// <summary>
    /// Loads all the resources that are in the chunk.
    /// </summary>
    private void LoadResources()
    {
        ResourcePoint[] resourcePoints = TerrainChunk.DataMap.ResourceMap.resourcePoints;

        for (int i = 0; i < resourcePoints.Length; i++)
        {
            TerrainInfo terrainInfo = GetTerrainInfoFromIndex(resourcePoints[i].IndexX, resourcePoints[i].IndexZ);

            if (!terrainInfo.Layer.IsWater)
            {
                ChunkPart terrainChunkPart = GetChunkPartByIndex(resourcePoints[i].IndexX, resourcePoints[i].IndexZ);
                terrainChunkPart.AddResourcePoint(resourcePoints[i], terrainInfo.WorldPosition);
            }
        }
    }

    private void LoopThroughAllTerrainPointsExample()
    {
        int size = TerrainChunk.DataMap.UniformSize;

        float halfSize = size / 2f;

        for (int x = 0; x < size; x++)
        {
            for (int z = 0; z < size; z++)
            {
                TerrainInfo terrainInfo = GetTerrainInfoFromIndex(x, z);

                if (terrainInfo.Layer.IsWater)
                    Debug.DrawRay(terrainInfo.WorldPosition, Vector3.up * 5f, Color.blue);
                else
                    Debug.DrawRay(terrainInfo.WorldPosition, Vector3.up * 5f, Color.white);
            }
        }
    }

    /// <summary>
    /// Get terrain properties at the given world location.
    /// </summary>
    /// <param name="worldX">The X position in world space that lies within the TerrainChunk bounds.</param>
    /// <param name="worldZ">The Z position in world space that lies within the TerrainChunk bounds.</param>
    /// <returns>Biome properties etc.</returns>
    public TerrainInfo GetTerrainInfoFromWorldPoint(float worldX, float worldZ)
    {
        if (!Loaded)
        {
            Debug.LogError("TerrainChunk hasn't loaded");
            return new TerrainInfo();
        }
        
        Vector3 distance = new Vector3(worldX, 0f, worldZ) - new Vector3(transform.position.x, 0f, transform.position.z);

        float xPercentage = distance.x / TerrainChunk.Bounds.size.x * 100f + 50f; // +50 since distance from middle is on scale of 0 to 50 or 0 to -50
        float zPercentage = distance.z / TerrainChunk.Bounds.size.y * 100f + 50f; // +50 since distance from middle is on scale of 0 to 50 or 0 to -50
        
        int xIndex = Mathf.RoundToInt((TerrainChunk.DataMap.UniformSize - 1) / 100f * xPercentage); 
        int zIndex = TerrainChunk.DataMap.UniformSize - 1 - Mathf.RoundToInt((TerrainChunk.DataMap.UniformSize - 1) / 100f * zPercentage); // Inversed

        // These offsets are needed for the HeightMap, since that [,] is 2 longer in width and height because those are needed for normal calculations.
        int indexOffsetX = 0;
        int indexOffsetZ = 0;

        if (xPercentage < 50)
        {
            indexOffsetX = 0;
            indexOffsetZ = zPercentage < 50f ? 1 : 0;
        }
        else
        {
            indexOffsetX = 1;
            indexOffsetZ = zPercentage < 50f ? 1 : 0;
        }

        Biome biome = TerrainChunk.DataMap.BiomeMap.GetBiome(xIndex, zIndex);
        HeightMapLayer layer = TerrainChunk.DataMap.GetLayer(xIndex + indexOffsetX, zIndex + indexOffsetZ);
        float height = TerrainChunk.DataMap.GetHeight(xIndex + indexOffsetX, zIndex + indexOffsetZ);

        Vector3 pos = transform.position + new Vector3((xIndex - TerrainChunk.DataMap.UniformSize / 2f) * TerrainChunk.MeshSettings.MeshScale, height, (zIndex - TerrainChunk.DataMap.UniformSize / 2f) * -TerrainChunk.MeshSettings.MeshScale);

        return new TerrainInfo(biome, layer, pos);
    }
    
    /// <summary>
     /// Get terrain properties at the given index.
     /// </summary>
     /// <param name="x">The X index.</param>
     /// <param name="z">The Z idnex.</param>
     /// <returns>Biome properties etc.</returns>
    public TerrainInfo GetTerrainInfoFromIndex(int x, int z)
    {
        if (!Loaded)
        {
            Debug.LogError("TerrainChunk hasn't loaded");
            return new TerrainInfo();
        }
        
        Biome biome = TerrainChunk.DataMap.GetBiome(x, z);
        float height = TerrainChunk.DataMap.GetHeight(x, z);
        HeightMapLayer layer = TerrainChunk.DataMap.GetLayer(x, z);

        Vector3 pos = transform.position + new Vector3((x - (TerrainChunk.DataMap.UniformSize - 1) / 2f) * TerrainChunk.MeshSettings.MeshScale, height, (z - (TerrainChunk.DataMap.UniformSize - 1) / 2f) * -TerrainChunk.MeshSettings.MeshScale);
        
        return new TerrainInfo(biome, layer, pos);
    }

    public class ChunkPart
    {
        public readonly Vector2 Coord;
        public readonly Vector3 WorldPosition;

        private readonly TerrainChunkController terrainChunkController;

        private readonly Dictionary<double, ResourcePoint> resourcePoints = new Dictionary<double, ResourcePoint>();
        private readonly Dictionary<double, Vector3> resourcePointPositions = new Dictionary<double, Vector3>();

        private readonly List<GameObject> spawnedInstances = new List<GameObject>();
        
        private bool isVisible;
        public bool IsVisible
        {
            get { return isVisible; }
            set
            {
                if (isVisible != value)
                {
                    isVisible = value;
                    if (isVisible)
                        SpawnResources();
                    else
                        DespawnResources();
                }
            }
                
        }
        
        public ChunkPart(Vector2 coord, Vector3 worldPosition, TerrainChunkController terrainChunkController)
        {
            this.Coord = coord;
            this.WorldPosition = worldPosition;
            this.terrainChunkController = terrainChunkController;
        }

        public void AddResourcePoint(ResourcePoint resourcePoint, Vector3 resourcePointPosition)
        {
            int a = resourcePoint.IndexX;
            int b = resourcePoint.IndexZ;
            double id = 0.5 * (a + b)*(a + b + 1) + b;

            if (!this.resourcePoints.ContainsKey(id))
            {
                this.resourcePoints.Add(id, resourcePoint);
                this.resourcePointPositions.Add(id, resourcePointPosition);
            }
            else
                Debug.LogError($"An ResourcePoint with the same position: `{resourcePointPosition}` already exists.");
        }

        private void SpawnResources()
        {
            foreach (var resourcePoint in resourcePoints)
            {
                spawnedInstances.Add(Instantiate(resourcePoint.Value.WorldResourcePrefab, resourcePointPositions[resourcePoint.Key], Quaternion.identity, terrainChunkController.transform));
            }
        }

        private void DespawnResources()
        {
            for (int i = spawnedInstances.Count - 1; i >= 0; i--)
                Destroy(spawnedInstances[i]);
            spawnedInstances.Clear();
        }
    }

    #region Debugging
    
    private void OnMouseDown()
    {
        Ray mouseRay = PlayerNetwork.PlayerObject.GetComponent<PlayerCameraController>().CameraReference.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f));
        RaycastHit raycastHitInfo;
        if (Physics.Raycast(mouseRay, out raycastHitInfo))
        {
            TerrainInfo terrainInfo = GetTerrainInfoFromWorldPoint(raycastHitInfo.point.x, raycastHitInfo.point.z);

            Debug.Log($"BiomeValue: {terrainInfo.Biome.Name}, Height: {terrainInfo.WorldPosition.y}, Actual:{raycastHitInfo.point.y}");
        }
    }
    #endregion //Debugging
}

public struct TerrainInfo
{
    public readonly Biome Biome;
    public readonly HeightMapLayer Layer;
    public readonly Vector3 WorldPosition;

    public TerrainInfo(Biome biome, HeightMapLayer heightMapLayer, Vector3 point)
    {
        this.Biome = biome;
        this.Layer = heightMapLayer;
        this.WorldPosition = point;
    }
}
