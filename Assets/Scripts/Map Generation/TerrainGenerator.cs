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

        public static int Seed { get; private set; }
        public static string WorldDataPath => PlayerSaveDataExchanger.Instance.WorldDataPath;

        [SerializeField]
        private int colliderLODIndex;
        public LODInfo[] detailLevels;

        public MeshSettings MeshSettings;
        public HeightMapSettings HeightMapSettings;
        public BiomeMapSettings BiomeMapSettings;
        public ResourceMapSettings ResourceMapSettings;

        public Material TerrainMeshMaterial;


        private TerrainViewer primaryViewer;
        [SerializeField]
        private List<TerrainViewer> secondaryViewers;

        private NavMeshSurface navMeshSurface;

        private float meshWorldSize;
        private int chunksVisibleInViewDistance;

        private static Dictionary<Vector2, TerrainChunk> terrainChunkDictionary;
        public static TerrainChunk GetTerrainChunk(Vector2 coord)
        {
            if (terrainChunkDictionary.ContainsKey(coord))
                return terrainChunkDictionary[coord];
            else
            {
                Debug.LogError("TerrainChunk does not exist");
            }
            return null;
        }
        
        private List<TerrainChunk> visibleTerrainChunks;

        private bool setupFinished;

        public bool IsBuildingNavmesh { get; private set; }

        private void Awake()
        {
            navMeshSurface = GetComponent<NavMeshSurface>();

            terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();
            visibleTerrainChunks = new List<TerrainChunk>();

            setupFinished = false;
            IsBuildingNavmesh = false;
        }

        private void Start()
        {
            ApplySeed();
        }

        private void Update()
        {
            if (!setupFinished)
                return;

            primaryViewer.Position = new Vector2(primaryViewer.Transform.position.x, primaryViewer.Transform.position.z);

            bool secondaryViewerMoveTresholdReached = false;
            foreach (var viewer in secondaryViewers)
            {
                if (viewer.Transform == null)
                    continue;

                viewer.Position = new Vector2(viewer.Transform.position.x, viewer.Transform.position.z);

                if ((viewer.PositionOld_ChunkUpdate - viewer.Position).sqrMagnitude > sqrViewerMoveThresholdForChunkUpdate)
                {
                    viewer.PositionOld_ChunkUpdate = viewer.Position;
                    secondaryViewerMoveTresholdReached = true;
                }
            }

            // Update collision meshes for all visibleTerrainChunks if the viewer has moved at all since the previous frame.
            if (primaryViewer.Position != primaryViewer.PositionOld_ChunkUpdate)
            {
                foreach (TerrainChunk terrainChunk in visibleTerrainChunks)
                    terrainChunk.UpdateCollisionMesh();
            }

            // Update all visible terrain chunks if the viewer has moved by a certain amount
            if ((primaryViewer.PositionOld_ChunkUpdate - primaryViewer.Position).sqrMagnitude > sqrViewerMoveThresholdForChunkUpdate || secondaryViewerMoveTresholdReached)
            {
                primaryViewer.PositionOld_ChunkUpdate = primaryViewer.Position;
                UpdateVisibleChunks();
            }

            // Update chunk parts of all the visible terrain chunks if the viewer has moved by a certain amount
            if ((primaryViewer.PositionOld_ChunkPartUpdate - primaryViewer.Position).sqrMagnitude > sqrViewerMoveThresholdForChunkPartUpdate)
            {
                primaryViewer.PositionOld_ChunkPartUpdate = primaryViewer.Position;

                foreach (TerrainChunk terrainChunk in visibleTerrainChunks)
                    terrainChunk.UpdateTerrainChunkParts();
            }
        }

        /// <summary>
        /// Setup is called when the seed is set through the SetSeed RPC call
        /// </summary>
        private void Setup()
        {
            HeightMapSettings.UpdateMeshHeights(TerrainMeshMaterial, HeightMapSettings.MinHeight, HeightMapSettings.MaxHeight);
            HeightMapSettings.ApplyToMaterial(TerrainMeshMaterial);

            float maxViewDistance = detailLevels[detailLevels.Length - 1].VisibleDistanceThreshold;
            meshWorldSize = MeshSettings.MeshWorldSize;
            chunksVisibleInViewDistance = Mathf.RoundToInt(maxViewDistance / meshWorldSize);

            primaryViewer = new TerrainViewer(PlayerNetwork.LocalPlayer.transform, TerrainViewer.ViewerTypes.primary);
            secondaryViewers = new List<TerrainViewer>();

            UpdateVisibleChunks();

            setupFinished = true;
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

            for (int yOffset = -1; yOffset <= 1; yOffset++)
            {
                for (int xOffset = -1; xOffset <= 1; xOffset++)
                {
                    // Find new chunks based on secondary viewer positions
                    foreach (var secondaryViewer in secondaryViewers)
                    {
                        int secondaryViewerChunkCoordX = Mathf.RoundToInt(secondaryViewer.Position.x / meshWorldSize);
                        int secondaryViewerChunkCoordY = Mathf.RoundToInt(secondaryViewer.Position.y / meshWorldSize);

                        Vector2 viewedChunkCoord = new Vector2(secondaryViewerChunkCoordX + xOffset, secondaryViewerChunkCoordY + yOffset);
                        if (!alreadyUpdatedChunkCoords.Contains(viewedChunkCoord))
                        {
                            if (terrainChunkDictionary.ContainsKey(viewedChunkCoord))
                            {
                                TerrainChunk terrainChunk = terrainChunkDictionary[viewedChunkCoord];
                                terrainChunk.Viewer = secondaryViewer;
                            }
                            else
                            {
                                TerrainChunk newChunk = new TerrainChunk(viewedChunkCoord, HeightMapSettings, BiomeMapSettings, ResourceMapSettings, MeshSettings, detailLevels, colliderLODIndex, transform, secondaryViewer, TerrainMeshMaterial);
                                terrainChunkDictionary.Add(viewedChunkCoord, newChunk);
                                newChunk.OnVisibilityChanged += OnTerrainChunkVisibilityChanged;
                                newChunk.OnColliderChanged += OnTerrainChunkColliderChanged;
                                newChunk.Load();
                            }
                        }
                    }
                }
            }

            // Find new chunks based on primary viewer position
            int primaryViewerChunkCoordX = Mathf.RoundToInt(primaryViewer.Position.x / meshWorldSize);
            int primaryViewerChunkCoordY = Mathf.RoundToInt(primaryViewer.Position.y / meshWorldSize);

            for (int yOffset = -chunksVisibleInViewDistance; yOffset <= chunksVisibleInViewDistance; yOffset++)
            {
                for (int xOffset = -chunksVisibleInViewDistance; xOffset <= chunksVisibleInViewDistance; xOffset++)
                {
                    Vector2 viewedChunkCoord = new Vector2(primaryViewerChunkCoordX + xOffset, primaryViewerChunkCoordY + yOffset);
                    if (!alreadyUpdatedChunkCoords.Contains(viewedChunkCoord))
                    {
                        if (terrainChunkDictionary.ContainsKey(viewedChunkCoord))
                        {
                            TerrainChunk terrainChunk = terrainChunkDictionary[viewedChunkCoord];
                            terrainChunk.Viewer = primaryViewer;

                            terrainChunk.UpdateTerrainChunk();
                        }
                        else
                        {
                            TerrainChunk newChunk = new TerrainChunk(viewedChunkCoord, HeightMapSettings, BiomeMapSettings, ResourceMapSettings, MeshSettings, detailLevels, colliderLODIndex, transform, primaryViewer, TerrainMeshMaterial);
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
        private void OnTerrainChunkColliderChanged(TerrainChunk chunk, bool hasSetCollider)
        {
            if (hasSetCollider)
            {
                if (IsBuildingNavmesh)
                    StopCoroutine(BuildNavMeshAsync());
                StartCoroutine(BuildNavMeshAsync());
            }
        }

        /// <summary>
        /// Builds the NavMesh based on current active chunks that have colliders active.
        /// </summary>
        private IEnumerator BuildNavMeshAsync()
        {
            IsBuildingNavmesh = true;
            Debug.Log("Started building navmesh");

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

            Debug.Log("Finished building navmesh");
            IsBuildingNavmesh = false;
        }

        /// <summary>
        /// Creates the NavMeshData
        /// </summary>
        /// <param name="surface"></param>
        /// <returns></returns>
        private NavMeshData InitializeBakeData()
        {
            List<NavMeshBuildSource> buildSources = new List<NavMeshBuildSource>();
            Bounds bounds = new Bounds(primaryViewer.Transform.position, new Vector3(100f, 50f, 100f));

            NavMeshBuilder.CollectSources(transform, navMeshSurface.layerMask, NavMeshCollectGeometry.PhysicsColliders, 0, new List<NavMeshBuildMarkup>(), buildSources);
            Debug.Log("Sources found: " + buildSources.Count.ToString());

            /*
            // Reduce the amount by excluding objects that are further than 100 units away. EXPERIMENTAL
            for (int i = buildSources.Count - 1; i >= 0; i--)
            {
                Vector3 position = buildSources[i].transform.GetColumn(3);
                if (Vector3.Distance(position, primaryViewer.Transform.position) > 100f)
                {
                    buildSources.Remove(buildSources[i]);
                }
            }
            Debug.Log("Reduced to: " + buildSources.Count.ToString()); */

            return NavMeshBuilder.BuildNavMeshData(navMeshSurface.GetBuildSettings(), buildSources, bounds, navMeshSurface.transform.position, navMeshSurface.transform.rotation);
        }

        private void OnWorldLoaded(bool loaded)
        {
            if (loaded)
                Setup();
        }
        
        private void ApplySeed()
        {
            Seed = (int)PhotonNetwork.room.CustomProperties["seed"];
            HeightMapSettings.NoiseSettings.seed = Seed;
            BiomeMapSettings.NoiseSettings.seed = Seed;
            ResourceMapSettings.NoiseSettings.seed = Seed;

            if (PlayerSaveDataExchanger.Instance.IsWorldDownloaded == false)
                PlayerSaveDataExchanger.Instance.OnWorldDownloaded += OnWorldLoaded;
            else
                OnWorldLoaded(true);
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

    [System.Serializable]
    public class TerrainViewer
    {
        public enum ViewerTypes { primary, secondary }
        public ViewerTypes ViewerType;

        public Transform Transform;
        public Vector2 Position;
        public Vector2 PositionOld_ChunkUpdate;
        public Vector2 PositionOld_ChunkPartUpdate;

        public TerrainViewer(Transform transform, ViewerTypes viewerType)
        {
            this.Transform = transform;
            this.ViewerType = viewerType;
        }
    }
}