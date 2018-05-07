using UnityEngine;

/// <summary>
/// Controller for a specific <see cref="global::TerrainChunk"/> instance.
/// </summary>
public class TerrainChunkController : MonoBehaviour
{
    private TerrainChunk terrainChunk;
    public TerrainChunk TerrainChunk
    {
        get { return this.terrainChunk; }
        set
        {
            this.terrainChunk = value;
            debugRayPosition = new Vector3(terrainChunk.SampleCenter.x, transform.position.y, terrainChunk.SampleCenter.y) * terrainChunk.MeshSettings.MeshScale;
        }
    }

    private bool resourcesSpawned = false;
    
    private void Update()
    {
        if (!resourcesSpawned && terrainChunk != null && terrainChunk.IsVisible)
            SpawnRecources();
    }

    private void SpawnRecources()
    {
        ResourcePoint[] resourcePoints = terrainChunk.DataMap.ResourceMap.resourcePoints;

        for (int i = 0; i < resourcePoints.Length; i++)
        {
            TerrainInfo terrainInfo = GetTerrainInfoFromIndex(resourcePoints[i].IndexX, resourcePoints[i].IndexZ);

            if (terrainInfo.Layer != null && !terrainInfo.Layer.IsWater)
            {
                WorldResourceManager.Instance.AddWorldResource(resourcePoints[i].WorldResourcePrefab, this, terrainInfo);
            }
        }

        resourcesSpawned = true;
    }

    private void LoopThroughAllTerrainPointsExample()
    {
        int size = terrainChunk.DataMap.UniformSize;

        float halfSize = size / 2f;

        for (int x = 0; x < size; x++)
        {
            for (int z = 0; z < size; z++)
            {
                Vector3 pos = transform.position + new Vector3((x - halfSize) * terrainChunk.MeshSettings.MeshScale, 0f, (z - halfSize) * -terrainChunk.MeshSettings.MeshScale);
                TerrainInfo terrainInfo = GetTerrainInfoFromWorldPoint(pos.x, pos.z);

                if (terrainInfo.Layer.IsWater)
                    Debug.DrawRay(terrainInfo.WorldPoint, Vector3.up * 2f, Color.blue);
                else
                    Debug.DrawRay(terrainInfo.WorldPoint, Vector3.up * 2f, Color.red);
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
        if (terrainChunk == null)
        {
            Debug.LogError("TerrainChunk hasn't loaded");
            return new TerrainInfo();
        }
        
        Vector3 distance = new Vector3(worldX, 0f, worldZ) - new Vector3(transform.position.x, 0f, transform.position.z);

        float xPercentage = distance.x / terrainChunk.Bounds.size.x * 100f + 50f; // +50 since distance from middle is on scale of 0 to 50 or 0 to -50
        float zPercentage = distance.z / terrainChunk.Bounds.size.y * 100f + 50f; // +50 since distance from middle is on scale of 0 to 50 or 0 to -50
        
        int xIndex = Mathf.RoundToInt((terrainChunk.DataMap.UniformSize - 1) / 100f * xPercentage); 
        int zIndex = terrainChunk.DataMap.UniformSize - 1 - Mathf.RoundToInt((terrainChunk.DataMap.UniformSize - 1) / 100f * zPercentage); // Inversed

        // These offsets are needed because our mesh has more vertices than that are actually shown on the terrain. 
        // This is because those extra vertices are used in Normal calculations. Only needed for the heightmap.
        int indexOffsetX = 0;
        int indexOffsetZ = 0;

        if (xPercentage < 50)
        {
            indexOffsetX = zPercentage < 50f ? 0 : 0;
            indexOffsetZ = zPercentage < 50f ? 1 : 0;
        }
        else
        {
            indexOffsetX = zPercentage < 50f ? 1 : 1;
            indexOffsetZ = zPercentage < 50f ? 1 : 0;
        }

        Biome biome = terrainChunk.DataMap.BiomeMap.GetBiome(xIndex, zIndex);
        HeightMapLayer layer = terrainChunk.DataMap.GetLayer(xIndex + indexOffsetX, zIndex + indexOffsetZ);
        float height = terrainChunk.DataMap.GetHeight(xIndex + indexOffsetX, zIndex + indexOffsetZ);
        
        return new TerrainInfo(biome, layer, new Vector3(worldX, height, worldZ));
    }
    
    /// <summary>
     /// Get terrain properties at the given index.
     /// </summary>
     /// <param name="x">The X index.</param>
     /// <param name="z">The Z idnex.</param>
     /// <returns>Biome properties etc.</returns>
    public TerrainInfo GetTerrainInfoFromIndex(int x, int z)
    {
        if (terrainChunk == null)
        {
            Debug.LogError("TerrainChunk hasn't loaded");
            return new TerrainInfo();
        }
        
        Biome biome = terrainChunk.DataMap.GetBiome(x, z);
        float height = terrainChunk.DataMap.GetHeight(x, z);// These offsets are needed because our mesh has more vertices than that are actually shown on the terrain. This is because those extra vertices are used in Normal calculations. Only needed for the heightmap.
        HeightMapLayer layer = terrainChunk.DataMap.GetLayer(x, z);

        Vector3 pos = transform.position + new Vector3((x - terrainChunk.DataMap.UniformSize / 2f) * terrainChunk.MeshSettings.MeshScale, 0f, (z - terrainChunk.DataMap.UniformSize / 2f) * -terrainChunk.MeshSettings.MeshScale);
        
        return new TerrainInfo(biome, layer, new Vector3(pos.x, height, pos.z));
    }

    #region Debugging

    private Vector3 debugRayPosition = Vector3.zero;

    private void OnMouseDown()
    {
        Ray mouseRay = PlayerNetwork.PlayerObject.GetComponent<PlayerCameraController>().CameraReference.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f));
        RaycastHit raycastHitInfo;
        if (Physics.Raycast(mouseRay, out raycastHitInfo))
        {
            debugRayPosition = raycastHitInfo.point;

            TerrainInfo terrainInfo = GetTerrainInfoFromWorldPoint(raycastHitInfo.point.x, raycastHitInfo.point.z);

            Debug.Log($"BiomeValue: {terrainInfo.Biome.Name}, Height: {terrainInfo.WorldPoint.y}, Actual:{raycastHitInfo.point.y}");
        }
    }
    #endregion //Debugging
}

public struct TerrainInfo
{
    public readonly Biome Biome;
    public readonly HeightMapLayer Layer;
    public readonly Vector3 WorldPoint;

    public TerrainInfo(Biome biome, HeightMapLayer heightMapLayer, Vector3 point)
    {
        this.Biome = biome;
        this.Layer = heightMapLayer;
        this.WorldPoint = point;
    }
}
