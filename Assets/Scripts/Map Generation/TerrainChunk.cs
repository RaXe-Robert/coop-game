using UnityEngine;
using System.Collections;

public class TerrainChunk
{
    private const float colliderGenerationDistanceThreshold = 5;

    public event System.Action<TerrainChunk, bool> OnVisibilityChanged;
    public Vector2 Coord { get; private set; }

    public MapData MapData { get; private set; }
    public bool MapDataReceived { get; private set; }

    private GameObject meshObject;
    public Vector2 SampleCenter { get; private set; }
    public Bounds Bounds { get; private set; }

    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;
    private MeshCollider meshCollider;

    private GameObject biomeTextureObject;
    private Renderer biomeTextureRenderer;
    private Texture2D biomeTextureMap;
    private bool biomeTextureMapReceived;

    private LODInfo[] detailLevels;
    private LODMesh[] lodMeshes;
    private int colliderLODIndex;
    
    private int previousLODIndex = -1;
    private bool hasSetCollider;
    private float maxViewDistance;

    public readonly HeightMapSettings HeightMapSettings;
    public readonly BiomeMapSettings BiomeMapSettings;
    public readonly MeshSettings MeshSettings;
    private Transform viewer;

    private Vector2 ViewerPosition => new Vector2(viewer.position.x, viewer.position.z);

    public bool IsVisible => meshObject.activeSelf;
    public void SetVisible(bool visible)
    {
        meshObject.SetActive(visible);
    }

    public TerrainChunk(Vector2 coord, HeightMapSettings heightMapSettings, MeshSettings meshSettings, BiomeMapSettings biomeMapSettings, LODInfo[] detailLevels, int colliderLODIndex, Transform parent, Transform viewer, Material terrainMeshMaterial, Material terrainMaterial)
    {
        this.Coord = coord;
        this.detailLevels = detailLevels;
        this.colliderLODIndex = colliderLODIndex;
        this.HeightMapSettings = heightMapSettings;
        this.MeshSettings = meshSettings;
        this.BiomeMapSettings = biomeMapSettings;
        this.viewer = viewer;

        SampleCenter = coord * meshSettings.MeshWorldSize / meshSettings.MeshScale;
        Vector2 position = coord * meshSettings.MeshWorldSize;
        Bounds = new Bounds(position, Vector2.one * meshSettings.MeshWorldSize);

        meshObject = new GameObject("Terrain Chunk");
        meshRenderer = meshObject.AddComponent<MeshRenderer>();
        meshFilter = meshObject.AddComponent<MeshFilter>();
        meshCollider = meshObject.AddComponent<MeshCollider>();
        meshObject.AddComponent<TerrainChunkInteraction>().SetTerrainChunk = this;
        meshRenderer.material = terrainMeshMaterial;

        meshObject.transform.position = new Vector3(position.x, 0, position.y);
        meshObject.transform.parent = parent;

        if (TerrainGenerator.DrawBiomes)
        {
            biomeTextureObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
            biomeTextureObject.name = "Biome Texture";
            biomeTextureRenderer = biomeTextureObject.GetComponent<MeshRenderer>();
            biomeTextureRenderer.material = terrainMaterial;
            biomeTextureObject.transform.SetParent(meshObject.transform, false);

            biomeTextureObject.transform.localScale = (Vector3.one * meshSettings.MeshWorldSize) / 10;
            biomeTextureObject.transform.position += Vector3.up * 10f;
        }

        SetVisible(false);

        lodMeshes = new LODMesh[detailLevels.Length];
        for (int i = 0; i < detailLevels.Length; i++)
        {
            lodMeshes[i] = new LODMesh(detailLevels[i].Lod);
            lodMeshes[i].UpdateCallback += UpdateTerrainChunk;
            if (i == colliderLODIndex)
                lodMeshes[i].UpdateCallback += UpdateCollisionMesh;
        }

        maxViewDistance = detailLevels[detailLevels.Length - 1].VisibleDistanceThreshold;
    }

    public void Load()
    {
        ThreadedDataRequester.RequestData(() => MapDataGenerator.GenerateDataMap(MeshSettings.NumVertsPerLine, HeightMapSettings, BiomeMapSettings, SampleCenter), OnMapDataReceived);
    }
    
    private void OnMapDataReceived(object mapDataObject)
    {
        this.MapData = (MapData)mapDataObject;
        MapDataReceived = true;

        if (TerrainGenerator.DrawBiomes)
        {
            biomeTextureMap = TextureGenerator.TextureFromBiomeMap(MapData.BiomeMap);
            biomeTextureMapReceived = true;

            UpdateTerrainChunk();
        }

        UpdateTerrainChunk();
    }

    public void UpdateTerrainChunk()
    {
        if (MapDataReceived)
        {
            float viewerDstFromNearestEdge = Mathf.Sqrt(Bounds.SqrDistance(ViewerPosition));

            bool wasVisible = IsVisible;
            bool visible = viewerDstFromNearestEdge <= maxViewDistance;

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

                if (lodIndex != previousLODIndex)
                {
                    LODMesh lodMesh = lodMeshes[lodIndex];
                    if (lodMesh.hasMesh)
                    {
                        previousLODIndex = lodIndex;
                        meshFilter.mesh = lodMesh.mesh;
                    }
                    else if (!lodMesh.hasRequestedMesh)
                        lodMesh.RequestMesh(MapData.HeightMap, MeshSettings);
                }

                if (biomeTextureMapReceived)
                {
                    biomeTextureRenderer.material.mainTexture = biomeTextureMap;
                }
            }

            if (wasVisible != visible)
            {
                SetVisible(visible);
                OnVisibilityChanged?.Invoke(this, visible);
            }
        }
    }

    public void UpdateCollisionMesh()
    {
        if (!hasSetCollider)
        {
            float sqrDstFromViewerToEdge = Bounds.SqrDistance(ViewerPosition);

            if (sqrDstFromViewerToEdge < detailLevels[colliderLODIndex].SqrVisibleDistanceThreshold)
            {
                if (!lodMeshes[colliderLODIndex].hasRequestedMesh)
                    lodMeshes[colliderLODIndex].RequestMesh(MapData.HeightMap, MeshSettings);
            }

            if (sqrDstFromViewerToEdge < colliderGenerationDistanceThreshold * colliderGenerationDistanceThreshold)
            {
                if (lodMeshes[colliderLODIndex].hasMesh)
                {
                    meshCollider.sharedMesh = lodMeshes[colliderLODIndex].mesh;
                    hasSetCollider = true;
                }
            }
        }
    }
}

public class LODMesh
{
    public Mesh mesh;
    public bool hasRequestedMesh;
    public bool hasMesh;
    private int lod;

    public event System.Action UpdateCallback;

    public LODMesh(int lod)
    {
        this.lod = lod;
    }

    private void OnMeshDataReceived(object meshData)
    {
        mesh = ((MeshData)meshData).CreateMesh();
        hasMesh = true;

        UpdateCallback();
    }

    public void RequestMesh(HeightMap heightMap, MeshSettings meshSettings)
    {
        hasRequestedMesh = true;
        ThreadedDataRequester.RequestData(() => MeshGenerator.GenerateTerrainMesh(heightMap.values, meshSettings, lod), OnMeshDataReceived);
    }
}


