using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(PhotonView))]
public class TerrainGenerator : Photon.MonoBehaviour
{
    private const float viewerMoveThresholdForChunkUpdate = 25f;
    private const float sqrViewerMoveThreasholdForChunkUpdate = viewerMoveThresholdForChunkUpdate * viewerMoveThresholdForChunkUpdate;

    public int colliderLODIndex;
    public LODInfo[] detailLevels;

    public MeshSettings meshSettings;
    public HeightMapSettings heightMapSettings;
    public TextureData textureSettings;

    public Transform viewer;
    public Material terrainMaterial;

    private Vector2 viewerPosition;
    private Vector2 viewerPositionOld;

    private float meshWorldSize;
    private int chunksVisibleInViewDistance;

    private Dictionary<Vector2, TerrainChunk> terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();
    private List<TerrainChunk> visibleTerrainChunks = new List<TerrainChunk>();

    private int RandomSeed => (new System.Random()).Next(0, int.MaxValue);

    [PunRPC]
    public void SetSeed(int seed)
    {
        heightMapSettings.noiseSettings.seed = seed;
        UnityEngine.Debug.Log($"Received generation rpc, building map with seed: {heightMapSettings.noiseSettings.seed}");
    }

    private void Start()
    {
        if (PhotonNetwork.isMasterClient)
        {
            int seed = RandomSeed;
            photonView.RPC("SetSeed", PhotonTargets.AllBuffered, seed);
        }

        textureSettings.ApplyToMaterial(terrainMaterial);
        textureSettings.UpdateMeshHeights(terrainMaterial, heightMapSettings.MinHeight, heightMapSettings.MaxHeight);
        
        float maxViewDistance = detailLevels[detailLevels.Length - 1].visibleDistanceThreshold;
        meshWorldSize = meshSettings.MeshWorldSize;
        chunksVisibleInViewDistance = Mathf.RoundToInt(maxViewDistance / meshWorldSize);

        viewer = PlayerNetwork.PlayerObject.transform;
        
        UpdateVisibleChunks();
    }

    private void Update()
    {
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
                    {
                        terrainChunkDictionary[viewedChunkCoord].UpdateTerrainChunk();
                    }
                    else
                    {
                        TerrainChunk newChunk = new TerrainChunk(viewedChunkCoord, heightMapSettings, meshSettings, detailLevels, colliderLODIndex, transform, viewer, terrainMaterial);
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
}

[System.Serializable]
public struct LODInfo
{
    [Range(0, MeshSettings.numSupportedLODs - 1)]
    public int lod;
    public float visibleDistanceThreshold;
    
    public float SqrVisibleDistanceThreshold
    {
        get { return visibleDistanceThreshold * visibleDistanceThreshold; }
    }
}
