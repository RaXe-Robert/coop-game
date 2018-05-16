using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts.Map_Generation
{
    /// <summary>
    /// This class takes a generated array of MapTiles and instantiates the propper planes and resources for the biomes.
    /// 
    /// The root is the gameobject that will recieve all the instantiated tiles and resources.
    /// The tree and rock prefabs are the resources that will be placed on the generated tiles.
    /// </summary>
    public class MapPreview : MonoBehaviour
    {
        public enum DrawMode { NoiseMap, Mesh, FalloffMap, BiomeMap, ResourceMap }
        public DrawMode drawMode;

        public MeshSettings MeshSettings;
        public HeightMapSettings HeightMapSettings;
        public BiomeMapSettings BiomeMapSettings;
        public ResourceMapSettings ResourceMapSettings;

        public Material TerrainMaterial;

        [Range(0, MeshSettings.MumOfSupportedLODs - 1)]
        public int editorPreviewLOD;

        public bool autoUpdate;

        [SerializeField]
        private Renderer textureRenderer;

        [SerializeField]
        private MeshFilter meshFilter;

        [SerializeField]
        private MeshRenderer meshRenderer;

        public void DrawMapInEditor()
        {
            HeightMapSettings.UpdateMeshHeights(TerrainMaterial, HeightMapSettings.MinHeight, HeightMapSettings.MaxHeight);
            HeightMapSettings.ApplyToMaterial(TerrainMaterial);

            HeightMap heightMap = HeightMapGenerator.GenerateHeightMap(MeshSettings.NumVertsPerLine, HeightMapSettings, Vector2.zero);

            switch (drawMode)
            {
                case DrawMode.NoiseMap:
                    DrawTexture(TextureGenerator.TextureFromHeightMap(heightMap));
                    break;
                case DrawMode.Mesh:
                    MeshData meshData = MeshGenerator.GenerateTerrainMesh(heightMap.Values, MeshSettings, editorPreviewLOD);
                    Texture2D texture = TextureGenerator.TextureFromBiomeMap(BiomeMapGenerator.GenerateBiomeMap(heightMap.Values.GetLength(0) - 3, BiomeMapSettings, Vector2.zero));
                    DrawMesh(meshData, texture);
                    break;
                case DrawMode.FalloffMap:
                    DrawTexture(TextureGenerator.TextureFromHeightMap(new HeightMap(FalloffGenerator.GenerateFalloffMap(MeshSettings.NumVertsPerLine), 0, 1, HeightMapSettings)));
                    break;
                case DrawMode.BiomeMap:
                    DrawTexture(TextureGenerator.TextureFromBiomeMap(BiomeMapGenerator.GenerateBiomeMap(heightMap.Values.GetLength(0) - 3, BiomeMapSettings, Vector2.zero)));
                    break;
                case DrawMode.ResourceMap:
                    DrawTexture(TextureGenerator.TextureFromObjectMap(ResourceMapGenerator.GenerateResourceMap(heightMap.Values.GetLength(0) - 3, ResourceMapSettings, Vector2.zero)));
                    break;

            }
        }

        /// <summary>
        /// Places all the tiles and resources according to the generated tileMap.
        /// </summary>
        /// <param name="noiseMap"></param>
        public void DrawTexture(Texture2D texture)
        {
            textureRenderer.gameObject.SetActive(true);
            meshFilter.gameObject.SetActive(false);

            textureRenderer.sharedMaterial.mainTexture = texture;
            textureRenderer.transform.localScale = new Vector3(texture.width, 1, texture.height) / 10f;
        }

        public void DrawMesh(MeshData meshData, Texture2D texture)
        {
            textureRenderer.gameObject.SetActive(false);
            meshFilter.gameObject.SetActive(true);

            meshFilter.sharedMesh = meshData.CreateMesh();
            meshRenderer.sharedMaterial.mainTexture = texture;
        }

        private void OnValuesUpdated()
        {
            if (!Application.isPlaying)
                DrawMapInEditor();
        }

        private void OnValidate()
        {
            if (MeshSettings != null)
            {
                MeshSettings.OnValuesUpdated -= OnValuesUpdated;
                MeshSettings.OnValuesUpdated += OnValuesUpdated;
            }
            if (HeightMapSettings != null)
            {
                HeightMapSettings.OnValuesUpdated -= OnValuesUpdated;
                HeightMapSettings.OnValuesUpdated += OnValuesUpdated;
            }
            if (BiomeMapSettings != null)
            {
                BiomeMapSettings.OnValuesUpdated -= OnValuesUpdated;
                BiomeMapSettings.OnValuesUpdated += OnValuesUpdated;
            }
            if (ResourceMapSettings != null)
            {
                ResourceMapSettings.OnValuesUpdated -= OnValuesUpdated;
                ResourceMapSettings.OnValuesUpdated += OnValuesUpdated;
            }
        }
    }
}
