using UnityEngine;
using System.Collections.Generic;
using System;

namespace Assets.Scripts.Map_Generation
{
    /// <summary>
    /// Chunk of the terrain.
    /// </summary>
    public class TerrainChunk
    {
        private const float colliderGenerationDistanceThreshold = 20;
        private const float sqrColliderGenerationDistanceThreshold = colliderGenerationDistanceThreshold * colliderGenerationDistanceThreshold;

        private const float chunkPartLoadDistanceThreshold = 9;
        private const float sqrChunkPartLoadDistanceThreshold = chunkPartLoadDistanceThreshold * chunkPartLoadDistanceThreshold;

        public event Action<TerrainChunk, bool> OnVisibilityChanged;
        public event Action<TerrainChunk, bool> OnColliderChanged;
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

        public TerrainViewer Viewer { get; set; }
        public Vector2 ViewerPosition => new Vector2(Viewer.Transform.position.x, Viewer.Transform.position.z);

        public bool IsVisible => MeshObject.activeSelf;
        public void SetVisible(bool visible)
        {
            // Clean up any visible chunkparts
            if (DataMapReceived)
            {
                foreach (TerrainChunkPart chunkPart in DataMap.ChunkParts.Values)
                {
                    if (chunkPart.Visible)
                        chunkPart.Visible = false;
                }
            }

            MeshObject.SetActive(visible);

            meshCollider.sharedMesh = null;
            HasSetCollider = false;
            OnColliderChanged?.Invoke(this, false);
        }

        public TerrainChunk(Vector2 coord, HeightMapSettings heightMapSettings, BiomeMapSettings biomeMapSettings, ResourceMapSettings resourceMapSettings, MeshSettings meshSettings, LODInfo[] detailLevels, int colliderLODIndex, Transform parent, TerrainViewer viewer, Material terrainMeshMaterial)
        {
            this.Coord = coord;
            this.HeightMapSettings = heightMapSettings;
            this.BiomeMapSettings = biomeMapSettings;
            this.ResourceMapSettings = resourceMapSettings;
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

            MeshObject.AddComponent<TerrainChunkInteraction>().TerrainChunk = this;

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
            if (!DataMapReceived || Viewer?.ViewerType == TerrainViewer.ViewerTypes.secondary)
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

        /// <summary>
        /// Activates or deactives chunk parts based on the distance to the viewer.
        /// </summary>
        public void UpdateTerrainChunkParts()
        {
            if (!DataMapReceived || (Viewer?.ViewerType == TerrainViewer.ViewerTypes.secondary))
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
            if (HasSetCollider || (Viewer?.ViewerType == TerrainViewer.ViewerTypes.secondary))
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
                    OnColliderChanged?.Invoke(this, true);
                }
            }
        }

        /// <summary>
        /// Get a chunk part by DataMap index.
        /// </summary>
        /// <param name="x">The x position in the DataMap.</param>
        /// <param name="z">The z position in the DataMap.</param>
        public TerrainChunkPart GetChunkPart(int x, int z)
        {
            if (x < 0 || z < 0 || x >= DataMap.UniformSize || z >= DataMap.UniformSize)
                throw new IndexOutOfRangeException("Index was greater than the DataMap size.");

            int chunkPartSize = DataMap.UniformSize / MeshSettings.ChunkPartSizeRoot + 1;
            int coordX = Mathf.FloorToInt(x / chunkPartSize) - 1;
            int coordZ = Mathf.FloorToInt(z / chunkPartSize) - 1;

            Vector2 chunkPartCoords = Coord * MeshSettings.ChunkPartSizeRoot + new Vector2(coordX, -coordZ);

            return DataMap.ChunkParts[chunkPartCoords];
        }

        /// <summary>
        /// Saves all current objects to disk (POSSIBLE REWORK NEEDED)
        /// </summary>
        public void SaveChanges()
        {
            List<ObjectPoint> objectPoints = new List<ObjectPoint>();

            foreach (var chunkPart in DataMap.ChunkParts)
                objectPoints.AddRange(chunkPart.Value.ObjectPoints.Values);

            ObjectMapLoader.SaveObjectMap(this, objectPoints.ToArray());
        }
    }

    /// <summary>
    /// Container for mesh data that can be applied to a chunk.
    /// </summary>
    public class LODMesh
    {
        public Mesh Mesh { get; private set; }
        public bool HasRequestedMesh { get; private set; }
        public bool HasMesh { get; private set; }
        private readonly int lod;

        public event Action UpdateCallback;

        public LODMesh(int lod)
        {
            this.lod = lod;
        }

        private void OnMeshDataReceived(object meshData)
        {
            Mesh = ((MeshData)meshData).CreateMesh();
            HasMesh = true;

            UpdateCallback?.Invoke();
        }

        public void RequestMesh(HeightMap heightMap, MeshSettings meshSettings)
        {
            HasRequestedMesh = true;
            ThreadedDataRequester.RequestData(() => MeshGenerator.GenerateTerrainMesh(heightMap.Values, meshSettings, lod), OnMeshDataReceived);
        }
    }
}


