using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// This class takes a generated array of MapTiles and instantiates the propper planes and resources for the biomes.
/// 
/// The root is the gameobject that will recieve all the instantiated tiles and resources.
/// The tree and rock prefabs are the resources that will be placed on the generated tiles.
/// </summary>
public class MapPreview : MonoBehaviour
{
    public enum DrawMode { NoiseMap, Mesh, FalloffMap }
    public DrawMode drawMode;

    public MeshSettings meshSettings;
    public HeightMapSettings heightMapSettings;
    public TextureData textureSettings;

    public Material terrainMaterial;

    [Range(0, MeshSettings.numSupportedLODs - 1)]
    public int editorPreviewLOD;

    public bool autoUpdate;

    private NavMeshSurface navmesh;

    [SerializeField]
    private Renderer textureRenderer;

    [SerializeField]
    private MeshFilter meshFilter;

    [SerializeField]
    private MeshRenderer meshRenderer;

    private void Awake()
    {
        navmesh = GetComponent<NavMeshSurface>();
    }

    public void DrawMapInEditor()
    {
        textureSettings.ApplyToMaterial(terrainMaterial);
        textureSettings.UpdateMeshHeights(terrainMaterial, heightMapSettings.MinHeight, heightMapSettings.MaxHeight);

        HeightMap heightMap = HeightMapGenerator.GenerateHeightMap(meshSettings.NumVertsPerLine, meshSettings.NumVertsPerLine, heightMapSettings, Vector2.zero);
        
        switch (drawMode)
        {
            case DrawMode.NoiseMap:
                DrawTexture(TextureGenerator.TextureFromHeightMap(heightMap));
                break;
            case DrawMode.Mesh:
                DrawMesh(MeshGenerator.GenerateTerrainMesh(heightMap.values, meshSettings, editorPreviewLOD));
                break;
            case DrawMode.FalloffMap:
                DrawTexture(TextureGenerator.TextureFromHeightMap(new HeightMap(FalloffGenerator.GenerateFalloffMap(meshSettings.NumVertsPerLine), 0, 1)));
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

    public void DrawMesh(MeshData meshData)
    {
        textureRenderer.gameObject.SetActive(false);
        meshFilter.gameObject.SetActive(true);

        meshFilter.sharedMesh = meshData.CreateMesh();
    }

    private void OnValuesUpdated()
    {
        if (!Application.isPlaying)
            DrawMapInEditor();
    }

    private void OnTextureValuesUpdated()
    {
        textureSettings.ApplyToMaterial(terrainMaterial);
    }

    private void OnValidate()
    {
        if (meshSettings != null)
        {
            meshSettings.OnValuesUpdated -= OnValuesUpdated;
            meshSettings.OnValuesUpdated += OnValuesUpdated;
        }
        if (heightMapSettings != null)
        {
            heightMapSettings.OnValuesUpdated -= OnValuesUpdated;
            heightMapSettings.OnValuesUpdated += OnValuesUpdated;
        }
        if (textureSettings != null)
        {
            textureSettings.OnValuesUpdated -= OnValuesUpdated;
            textureSettings.OnValuesUpdated += OnValuesUpdated;
        }
    }
}
