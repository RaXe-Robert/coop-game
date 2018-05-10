using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(PhotonView))]
public class TerrainGenerator : Photon.MonoBehaviour
{
    private const float viewerMoveThresholdForChunkUpdate = 25f;
    private const float sqrViewerMoveThresholdForChunkUpdate = viewerMoveThresholdForChunkUpdate * viewerMoveThresholdForChunkUpdate;

    private const float viewerMoveThresholdForChunkPartUpdate = 5f;
    private const float sqrViewerMoveThresholdForChunkPartUpdate = viewerMoveThresholdForChunkPartUpdate * viewerMoveThresholdForChunkPartUpdate;

    [SerializeField]
    private int colliderLODIndex;
    public LODInfo[] detailLevels;

    public MeshSettings MeshSettings;
    public HeightMapSettings HeightMapSettings;
    public BiomeMapSettings BiomeMapSettings;
    public ResourceMapSettings ResourceMapSettings;

    public Material TerrainMeshMaterial;

    private Transform viewer;
    private Vector2 viewerPosition;
    private Vector2 viewerPositionOld_ChunkUpdate;
    private Vector2 viewerPositionOld_ChunkPartUpdate;

    private float meshWorldSize;
    private int chunksVisibleInViewDistance;

    private Dictionary<Vector2, TerrainChunk> terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();
    private List<TerrainChunk> visibleTerrainChunks = new List<TerrainChunk>();
    
    private bool isSeedSet = false;

    private void Start()
    {
        if (PhotonNetwork.isMasterClient)
            photonView.RPC("SetSeed", PhotonTargets.AllBuffered, (new System.Random()).Next(0, int.MaxValue));
    }

    private void Update()
    {
        if (!isSeedSet)
            return;

        viewerPosition = new Vector2(viewer.position.x, viewer.position.z);

        // Update collision meshes for all visibleTerrainChunks if the viewer has moved at all since the previous frame.
        if (viewerPosition != viewerPositionOld_ChunkUpdate)
        {
            foreach (TerrainChunk terrainChunk in visibleTerrainChunks)
                terrainChunk.UpdateCollisionMesh();
        }

        // Update all visible terrain chunks if the viewer has moved by a certain amount
        if ((viewerPositionOld_ChunkUpdate - viewerPosition).sqrMagnitude > sqrViewerMoveThresholdForChunkUpdate)
        {
            viewerPositionOld_ChunkUpdate = viewerPosition;
            UpdateVisibleChunks();
        }
        
        // Update chunk parts of all the visible terrain chunks if the viewer has moved by a certain amount
        if ((viewerPositionOld_ChunkPartUpdate - viewerPosition).sqrMagnitude > sqrViewerMoveThresholdForChunkPartUpdate || true)
        {
            viewerPositionOld_ChunkPartUpdate = viewerPosition;

            foreach (TerrainChunk terrainChunk in visibleTerrainChunks)
            {
                terrainChunk.UpdateTerrainChunkParts();
            }
        }
    }

    /// <summary>
    /// Setup is called when the seed is set through the SetSeed RPC call
    /// </summary>
    private void Setup()
    {
        isSeedSet = true;

        HeightMapSettings.UpdateMeshHeights(TerrainMeshMaterial, HeightMapSettings.MinHeight, HeightMapSettings.MaxHeight);
        HeightMapSettings.ApplyToMaterial(TerrainMeshMaterial);

        float maxViewDistance = detailLevels[detailLevels.Length - 1].VisibleDistanceThreshold;
        meshWorldSize = MeshSettings.MeshWorldSize;
        chunksVisibleInViewDistance = Mathf.RoundToInt(maxViewDistance / meshWorldSize);

        viewer = PlayerNetwork.PlayerObject.transform;

        UpdateVisibleChunks();
    }

    /// <summary>
    /// Update all visible chunks, tries to find new chunks aswell.
    /// </summary>
    private void UpdateVisibleChunks()
    {
        // Update the current visible chunks
        HashSet<Vector2> alreadyUpdatedChunkCoords = new HashSet<Vector2>();
        for (int i = visibleTerrainChunks.Count - 1; i >= 0; i--)
        {
            alreadyUpdatedChunkCoords.Add(visibleTerrainChunks[i].Coord);
            visibleTerrainChunks[i].UpdateTerrainChunk();
        }

        // Find new chunks based on viewer position
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
                        TerrainChunk newChunk = new TerrainChunk(viewedChunkCoord, HeightMapSettings, BiomeMapSettings, ResourceMapSettings, MeshSettings, detailLevels, colliderLODIndex, transform, viewer, TerrainMeshMaterial);
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