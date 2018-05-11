using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controller for a specific <see cref="global::TerrainChunk"/> instance. Manages the resources loading based on the data within the TerrainChunk.
/// </summary>
public class TerrainChunkController : MonoBehaviour
{
    public TerrainChunk TerrainChunk { get; set; }
    public bool Loaded => TerrainChunk?.DataMapReceived ?? false;
    
    private void LoopThroughAllTerrainPointsExample()
    {
        int size = TerrainChunk.DataMap.UniformSize;
        
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
