using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(PhotonView))]
public class TerrainGenerator : Photon.MonoBehaviour
{
    private const float viewerMoveThresholdForChunkUpdate = 25f;
    private const float sqrViewerMoveThreasholdForChunkUpdate = viewerMoveThresholdForChunkUpdate * viewerMoveThresholdForChunkUpdate;
    public static bool DrawBiomes = false;

    public int colliderLODIndex;
    public LODInfo[] detailLevels;

    public MeshSettings MeshSettings;
    public HeightMapSettings HeightMapSettings;
    public TextureData TextureSettings;
    public BiomeMapSettings BiomeMapSettings;

    public Material TerrainMeshMaterial;
    
    public Material TerrainMaterial;

    private Transform viewer;
    private Vector2 viewerPosition;
    private Vector2 viewerPositionOld;

    private float meshWorldSize;
    private int chunksVisibleInViewDistance;

    private Dictionary<Vector2, TerrainChunk> terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();
    private List<TerrainChunk> visibleTerrainChunks = new List<TerrainChunk>();
    
    private bool isSeedSet = false;

    private void Start()
    {
        if (PhotonNetwork.isMasterClient)
        {
            photonView.RPC("SetSeed", PhotonTargets.AllBuffered, (new System.Random()).Next(0, int.MaxValue));
        }
    }

    private void Update()
    {
        if (!isSeedSet)
            return;

        viewerPosition = new Vector2(viewer.position.x, viewer.position.z);

        if (viewerPosition != viewerPositionOld)
        {
            foreach (TerrainChunk terrainChunk in visibleTerrainChunks)
                terrainChunk.UpdateCollisionMesh();
        }

        if ((viewerPositionOld - viewerPosition).sqrMagnitude > sqrViewerMoveThreasholdForChunkUpdate)
        {
            viewerPositionOld = viewerPosition;
            UpdateVisibleChunks();
        }
    }

    private void Setup()
    {
        isSeedSet = true;

        TextureSettings.UpdateMeshHeights(TerrainMeshMaterial, HeightMapSettings.MinHeight, HeightMapSettings.MaxHeight);
        TextureSettings.ApplyToMaterial(TerrainMeshMaterial);

        float maxViewDistance = detailLevels[detailLevels.Length - 1].VisibleDistanceThreshold;
        meshWorldSize = MeshSettings.MeshWorldSize;
        chunksVisibleInViewDistance = Mathf.RoundToInt(maxViewDistance / meshWorldSize);

        viewer = PlayerNetwork.PlayerObject.transform;

        UpdateVisibleChunks();
    }

    private void UpdateVisibleChunks()
    {
        HashSet<Vector2> alreadyUpdatedChunkCoords = new HashSet<Vector2>();
        for (int i = visibleTerrainChunks.Count - 1; i >= 0; i--)
        {
            alreadyUpdatedChunkCoords.Add(visibleTerrainChunks[i].coord);
            visibleTerrainChunks[i].UpdateTerrainChunk();
        }

        int currentChunkCoordX = Mathf.RoundToInt(viewerPosition.x / meshWorldSize);
        int currentChunkCoordY = Mathf.RoundToInt(viewerPosition.y / meshWorldSize);

        for (int yOffset = -chunksVisibleInViewDistance; yOffset <= chunksVisibleInViewDistance; yOffset++)
        {
            for (int xOffset = -chunksVisibleInViewDistance; xOffset <= chunksVisibleInViewDistance; xOffset++)
            {
                Vector2 viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);
                if (!alreadyUpdatedChunkCoords.Contains(viewedChunkCoord))
                {
                    if (terrainChunkDictionary.ContainsKey(viewedChunkCoord))
                        terrainChunkDictionary[viewedChunkCoord].UpdateTerrainChunk();
                    else
                    {
                        TerrainChunk newChunk = new TerrainChunk(viewedChunkCoord, HeightMapSettings, MeshSettings, BiomeMapSettings, detailLevels, colliderLODIndex, transform, viewer, TerrainMeshMaterial, TerrainMaterial);
                        terrainChunkDictionary.Add(viewedChunkCoord, newChunk);
                        newChunk.OnVisibilityChanged += OnTerrainChunkVisibilityChanged;
                        newChunk.Load();
                    }
                }
            }
        }
    }

    private void OnTerrainChunkVisibilityChanged(TerrainChunk chunk, bool isVisible)
    {
        if (isVisible)
            visibleTerrainChunks.Add(chunk);
        else
            visibleTerrainChunks.Remove(chunk);
    }

    [PunRPC]
    public void SetSeed(int seed)
    {
        HeightMapSettings.NoiseSettings.seed = seed;
        UnityEngine.Debug.Log($"Received generation rpc, building map with seed: {HeightMapSettings.NoiseSettings.seed}");
        Setup();
    }
}

[System.Serializable]
public struct LODInfo
{
    [Range(0, MeshSettings.MumOfSupportedLODs - 1)]
    public int Lod;
    public float VisibleDistanceThreshold;
    
    public float SqrVisibleDistanceThreshold => VisibleDistanceThreshold * VisibleDistanceThreshold;
}
