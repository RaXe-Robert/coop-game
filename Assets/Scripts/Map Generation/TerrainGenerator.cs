using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

namespace Assets.Scripts.Map_Generation
{
    /// <summary>
    /// Class that handles the generation of the terrain by creating chunks etc.
    /// </summary>
    [RequireComponent(typeof(PhotonView), typeof(NavMeshSurface))]
    public class TerrainGenerator : Photon.MonoBehaviour
    {
        private const float viewerMoveThresholdForChunkUpdate = 25f;
        private const float sqrViewerMoveThresholdForChunkUpdate = viewerMoveThresholdForChunkUpdate * viewerMoveThresholdForChunkUpdate;

        private const float viewerMoveThresholdForChunkPartUpdate = 5f;
        private const float sqrViewerMoveThresholdForChunkPartUpdate = viewerMoveThresholdForChunkPartUpdate * viewerMoveThresholdForChunkPartUpdate;

        // Testing, refactor required
        private static string PersistentDataPath;
        private static int Seed;
        public static string WorldDataPath
        {
            get
            {
#if UNITY_EDITOR
                return $"{PersistentDataPath}/{Seed}/Editor";
#else
                return $"{PersistentDataPath}/{Seed}";
#endif
            }
        }

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

        private NavMeshSurface navMeshSurface;

        private float meshWorldSize;
        private int chunksVisibleInViewDistance;

        private static Dictionary<Vector2, TerrainChunk> terrainChunkDictionary;
        public static TerrainChunk GetTerrainChunk(Vector2 coord)
        {
            if (terrainChunkDictionary.ContainsKey(coord))
                return terrainChunkDictionary[coord];
            return null;
        }


        private List<TerrainChunk> visibleTerrainChunks;

        private bool isSeedSet = false;

        private void Awake()
        {
            navMeshSurface = GetComponent<NavMeshSurface>();

            PersistentDataPath = Application.persistentDataPath;

            terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();
            visibleTerrainChunks = new List<TerrainChunk>();
        }

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
            if ((viewerPositionOld_ChunkPartUpdate - viewerPosition).sqrMagnitude > sqrViewerMoveThresholdForChunkPartUpdate)
            {
                viewerPositionOld_ChunkPartUpdate = viewerPosition;

                foreach (TerrainChunk terrainChunk in visibleTerrainChunks)
                    terrainChunk.UpdateTerrainChunkParts();
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
                            newChunk.OnColliderChanged += OnTerrainChunkColliderChanged;
                            newChunk.Load();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Responds to chunk visibility callbacks. Adds the chunk to a list of visible terrainChunks.
        /// </summary>
        /// <param name="chunk"></param>
        /// <param name="isVisible"></param>
        private void OnTerrainChunkVisibilityChanged(TerrainChunk chunk, bool isVisible)
        {
            if (isVisible)
                visibleTerrainChunks.Add(chunk);
            else
                visibleTerrainChunks.Remove(chunk);
        }

        /// <summary>
        /// Responds to chunk collider callbacks. Updates the navmesh based on the new collider data.
        /// </summary>
        /// <param name="chunk"></param>
        private void OnTerrainChunkColliderChanged(TerrainChunk chunk)
        {
            StartCoroutine(BuildNavMeshAsync());
        }

        /// <summary>
        /// Builds the NavMesh based on current active chunks that have colliders active.
        /// </summary>
        private IEnumerator BuildNavMeshAsync()
        {
            // Get the data for the surface
            NavMeshData data = InitializeBakeData();

            // Start building the navmesh
            AsyncOperation async = navMeshSurface.UpdateNavMesh(data);

            // Wait until the navmesh has finished baking
            yield return async;

            // Remove current data
            navMeshSurface.RemoveData();

            // Save the new data into the surface
            navMeshSurface.navMeshData = data;

            // Finalize / Apply
            navMeshSurface.AddData();

            Debug.Log("finished");
        }

        /// <summary>
        /// Creates the NavMeshData
        /// </summary>
        /// <param name="surface"></param>
        /// <returns></returns>
        private NavMeshData InitializeBakeData()
        {
            List<NavMeshBuildSource> buildSources = new List<NavMeshBuildSource>();
            Bounds bounds = new Bounds(viewer.position, new Vector3(100f, 50f, 100f));

            NavMeshBuilder.CollectSources(transform, navMeshSurface.layerMask, NavMeshCollectGeometry.PhysicsColliders, 0, new List<NavMeshBuildMarkup>(), buildSources);
            Debug.Log("Sources found: " + buildSources.Count.ToString());

            // Reduce the amount by excluding objects that are further than 100 units away. EXPERIMENTAL
            for (int i = buildSources.Count - 1; i >= 0; i--)
            {
                Vector3 position = buildSources[i].transform.GetColumn(3);
                if (Vector3.Distance(position, viewer.position) > 100f)
                {
                    buildSources.Remove(buildSources[i]);
                }
            }
            Debug.Log("Reduced to: " + buildSources.Count.ToString());

            return NavMeshBuilder.BuildNavMeshData(navMeshSurface.GetBuildSettings(), buildSources, bounds, navMeshSurface.transform.position, navMeshSurface.transform.rotation);
        }

        [PunRPC]
        public void SetSeed(int seed)
        {
            seed = 349260201;
            HeightMapSettings.NoiseSettings.seed = seed;
            BiomeMapSettings.NoiseSettings.seed = seed;
            ResourceMapSettings.NoiseSettings.seed = seed;

            Seed = seed;
            Debug.Log($"Received generation rpc, building map with seed: {Seed}");

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
}