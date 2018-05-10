using UnityEngine;
using System.Collections;

/// <summary>
/// Chunk of the terrain.
/// </summary>
public class TerrainChunk
{
    private const float colliderGenerationDistanceThreshold = 5;
    private const float sqrColliderGenerationDistanceThreshold = colliderGenerationDistanceThreshold * colliderGenerationDistanceThreshold;

    public event System.Action<TerrainChunk, bool> OnVisibilityChanged;
    public event System.Action<TerrainChunk, int> OnLODIndexChanged;
    public Vector2 Coord { get; private set; }

    public DataMap DataMap { get; private set; }
    public bool MapDataReceived { get; private set; }

    public readonly GameObject MeshObject;
    public readonly TerrainChunkController TerrainChunkController;
    public Vector2 SampleCenter { get; private set; }
    public Bounds Bounds { get; private set; }

    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;
    private MeshCollider meshCollider;
    
    private readonly LODInfo[] detailLevels;
    private readonly LODMesh[] lodMeshes;
    private readonly int colliderLODIndex;

    public int CurrentLODIndex { get; private set; } = -1;
    public bool HasSetCollider { get; private set; } = false;
    public float MaxViewDistance { get; private set; }

    public readonly HeightMapSettings HeightMapSettings;
    public readonly BiomeMapSettings BiomeMapSettings;
    public readonly ResourceMapSettings ResourceMapSettings;
    public readonly MeshSettings MeshSettings;

    public Transform Viewer { get; private set; }
    public Vector2 ViewerPosition => new Vector2(Viewer.position.x, Viewer.position.z);

    public bool IsVisible => MeshObject.activeSelf;
    public void SetVisible(bool visible)
    {
        MeshObject.SetActive(visible);
    }

    public TerrainChunk(Vector2 coord, HeightMapSettings heightMapSettings, BiomeMapSettings biomeMapSettings, ResourceMapSettings objectMapSettings, MeshSettings meshSettings, LODInfo[] detailLevels, int colliderLODIndex, Transform parent, Transform viewer, Material terrainMeshMaterial)
    {
        this.Coord = coord;
        this.HeightMapSettings = heightMapSettings;
        this.BiomeMapSettings = biomeMapSettings;
        this.ResourceMapSettings = objectMapSettings;
        this.MeshSettings = meshSettings;
        this.detailLevels = detailLevels;
        this.colliderLODIndex = colliderLODIndex;
        this.Viewer = viewer;

        SampleCenter = coord * meshSettings.MeshWorldSize / meshSettings.MeshScale;
        Vector2 position = coord * meshSettings.MeshWorldSize;
        Bounds = new Bounds(position, Vector2.one * meshSettings.MeshWorldSize);

        MeshObject = new GameObject("Terrain Chunk");
        meshRenderer = MeshObject.AddComponent<MeshRenderer>();
        meshFilter = MeshObject.AddComponent<MeshFilter>();
        meshCollider = MeshObject.AddComponent<MeshCollider>();
        meshRenderer.material = terrainMeshMaterial;

        TerrainChunkController = MeshObject.AddComponent<TerrainChunkController>();

        MeshObject.transform.position = new Vector3(position.x, 0, position.y);
        MeshObject.transform.parent = parent;
        MeshObject.layer = parent.gameObject.layer;
        
        SetVisible(false);

        lodMeshes = new LODMesh[detailLevels.Length];
        for (int i = 0; i < detailLevels.Length; i++)
        {
            lodMeshes[i] = new LODMesh(detailLevels[i].Lod);
            lodMeshes[i].UpdateCallback += UpdateTerrainChunk;
            if (i == colliderLODIndex)
                lodMeshes[i].UpdateCallback += UpdateCollisionMesh;
        }

        MaxViewDistance = detailLevels[detailLevels.Length - 1].VisibleDistanceThreshold;
    }

    public void Load()
    {
        ThreadedDataRequester.RequestData(() => DataMapGenerator.GenerateDataMap(MeshSettings.NumVertsPerLine, HeightMapSettings, BiomeMapSettings, ResourceMapSettings, SampleCenter), OnMapDataReceived);
    }
    
    private void OnMapDataReceived(object dataMapObject)
    {
        this.DataMap = (DataMap)dataMapObject;
        MapDataReceived = true;

        meshRenderer.material.mainTexture = TextureGenerator.TextureFromBiomeMap(DataMap.BiomeMap);

        TerrainChunkController.Load(this);

        UpdateTerrainChunk();
    }

    public void UpdateTerrainChunk()
    {
        if (!MapDataReceived)
            return;

        float viewerDstFromNearestEdge = Mathf.Sqrt(Bounds.SqrDistance(ViewerPosition));

        bool wasVisible = IsVisible;
        bool visible = viewerDstFromNearestEdge <= MaxViewDistance;

        if (visible)
        {
            int lodIndex = 0;

            for (int i = 0; i < detailLevels.Length - 1; i++)
            {
                if (viewerDstFromNearestEdge > detailLevels[i].VisibleDistanceThreshold)
                    lodIndex = i + 1;
                else
                    break;
            }

            if (lodIndex != CurrentLODIndex)
            {
                LODMesh lodMesh = lodMeshes[lodIndex];
                if (lodMesh.HasMesh)
                {
                    CurrentLODIndex = lodIndex;
                    meshFilter.mesh = lodMesh.Mesh;
                }
                else if (!lodMesh.HasRequestedMesh)
                    lodMesh.RequestMesh(DataMap.HeightMap, MeshSettings);
            }
        }

        if (wasVisible != visible)
        {
            SetVisible(visible);
            OnVisibilityChanged?.Invoke(this, visible);
        }
    }

    public void UpdateCollisionMesh()
    {
        if (HasSetCollider)
            return;

        float sqrDstFromViewerToEdge = Bounds.SqrDistance(ViewerPosition);

        if (sqrDstFromViewerToEdge < detailLevels[colliderLODIndex].SqrVisibleDistanceThreshold)
        {
            if (!lodMeshes[colliderLODIndex].HasRequestedMesh)
                lodMeshes[colliderLODIndex].RequestMesh(DataMap.HeightMap, MeshSettings);
        }

        if (sqrDstFromViewerToEdge < sqrColliderGenerationDistanceThreshold)
        {
            if (lodMeshes[colliderLODIndex].HasMesh)
            {
                meshCollider.sharedMesh = lodMeshes[colliderLODIndex].Mesh;
                HasSetCollider = true;
            }
        }
    }
}

public class LODMesh
{
    public Mesh Mesh { get; private set; }
    public bool HasRequestedMesh { get; private set; }
    public bool HasMesh { get; private set; }
    private readonly int lod;

    public event System.Action UpdateCallback;

    public LODMesh(int lod)
    {
        this.lod = lod;
    }

    private void OnMeshDataReceived(object meshData)
    {
        Mesh = ((MeshData)meshData).CreateMesh();
        HasMesh = true;

        UpdateCallback();
    }

    public void RequestMesh(HeightMap heightMap, MeshSettings meshSettings)
    {
        HasRequestedMesh = true;
        ThreadedDataRequester.RequestData(() => MeshGenerator.GenerateTerrainMesh(heightMap.Values, meshSettings, lod), OnMeshDataReceived);
    }
}


