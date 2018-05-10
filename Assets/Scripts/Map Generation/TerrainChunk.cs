using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// Chunk of the terrain.
/// </summary>
public class TerrainChunk
{
    private const float colliderGenerationDistanceThreshold = 5;
    private const float sqrColliderGenerationDistanceThreshold = colliderGenerationDistanceThreshold * colliderGenerationDistanceThreshold;

    private const float chunkPartLoadDistanceThreshold = 12;
    private const float sqrChunkPartLoadDistanceThreshold = chunkPartLoadDistanceThreshold * chunkPartLoadDistanceThreshold;

    public event System.Action<TerrainChunk, bool> OnVisibilityChanged;
    public Vector2 Coord { get; private set; }

    public DataMap DataMap { get; private set; }
    public bool DataMapReceived { get; private set; }
    
    public readonly GameObject MeshObject;

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
        // Clean up any visible chunkparts
        if (DataMapReceived)
        {
            foreach (ChunkPart chunkPart in DataMap.ChunkParts.Values)
            {
                if (chunkPart.Visible)
                    chunkPart.Visible = false;
            }
        }

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

        MeshObject.AddComponent<TerrainChunkController>().TerrainChunk = this;

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
        ThreadedDataRequester.RequestData(() => DataMapGenerator.GenerateDataMap(MeshSettings, HeightMapSettings, BiomeMapSettings, ResourceMapSettings, this), OnMapDataReceived);
    }
    
    private void OnMapDataReceived(object dataMapObject)
    {
        this.DataMap = (DataMap)dataMapObject;
        DataMapReceived = true;

        meshRenderer.material.mainTexture = TextureGenerator.TextureFromBiomeMap(DataMap.BiomeMap);

        UpdateTerrainChunk();
        UpdateTerrainChunkParts();
    }

    public void UpdateTerrainChunk()
    {
        if (!DataMapReceived)
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

    public void UpdateTerrainChunkParts()
    {
        if (!DataMapReceived)
            return;

        Vector2 viewerPosition = ViewerPosition;
        
        foreach (var chunkPartKVP in DataMap.ChunkParts)
        {

            float viewerDistanceFromChunkPart = Vector2.Distance(new Vector2(chunkPartKVP.Value.WorldPosition.x, chunkPartKVP.Value.WorldPosition.z), viewerPosition);

            bool wasVisible = chunkPartKVP.Value.Visible;
            bool visible = viewerDistanceFromChunkPart <= sqrChunkPartLoadDistanceThreshold;
            
            if (wasVisible != visible)
                chunkPartKVP.Value.Visible = visible;
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

    public ChunkPart GetChunkPart(int x, int z)
    {
        if (x < 0 || z < 0 || x >= DataMap.UniformSize || z >= DataMap.UniformSize)
            throw new System.IndexOutOfRangeException("Index was greater than the DataMap size.");

        int chunkPartSize = DataMap.UniformSize / MeshSettings.ChunkPartSizeRoot + 1;

        int coordX = Mathf.FloorToInt(x / chunkPartSize) - 1;
        int coordZ = Mathf.FloorToInt(z / chunkPartSize) - 1;

        Vector2 chunkPartCoords = Coord * MeshSettings.ChunkPartSizeRoot + new Vector2(coordX, -coordZ);

        return DataMap.ChunkParts[chunkPartCoords];
    }
}

public class ChunkPart
{
    public readonly Vector2 Coord;
    public readonly Vector3 WorldPosition;

    private readonly TerrainChunk terrainChunk;

    private readonly Dictionary<double, Tuple<ResourcePoint, Vector3>> resourcePoints = new Dictionary<double, Tuple<ResourcePoint, Vector3>>();

    private readonly List<GameObject> spawnedInstances = new List<GameObject>();

    private bool isVisible;
    public bool Visible
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

    public ChunkPart(Vector2 coord, Vector3 worldPosition, TerrainChunk terrainChunk)
    {
        this.Coord = coord;
        this.WorldPosition = worldPosition;
        this.terrainChunk = terrainChunk;
    }

    public void AddResourcePoint(ResourcePoint resourcePoint, Vector3 resourcePointPosition)
    {
        int a = resourcePoint.IndexX;
        int b = resourcePoint.IndexZ;
        double id = 0.5 * (a + b) * (a + b + 1) + b;

        if (!this.resourcePoints.ContainsKey(id))
            this.resourcePoints.Add(id, Tuple.Create(resourcePoint, resourcePointPosition));
        else
            Debug.LogError($"An ResourcePoint with the same position: `{resourcePointPosition}` already exists.");
    }

    private void SpawnResources()
    {
        foreach (var resourcePoint in resourcePoints)
            spawnedInstances.Add(UnityEngine.Object.Instantiate(resourcePoint.Value.Item1.WorldResourcePrefab, resourcePoint.Value.Item2, Quaternion.identity, terrainChunk.MeshObject.transform));
    }

    private void DespawnResources()
    {
        for (int i = spawnedInstances.Count - 1; i >= 0; i--)
            UnityEngine.Object.Destroy(spawnedInstances[i]);
        spawnedInstances.Clear();
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


