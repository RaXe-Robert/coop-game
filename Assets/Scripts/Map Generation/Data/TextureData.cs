using System.Linq;
using UnityEngine;

[CreateAssetMenu()]
public class TextureData : UpdatableData
{
    public Layer[] layers;

    private float savedMinHeight;
    private float savedMaxHeight;

    public void ApplyToMaterial(Material material)
    {
        material.SetInt("layerCount", layers.Length);
        material.SetColorArray("baseColors", layers.Select(x => x.tint).ToArray());
        material.SetFloatArray("baseStartHeights", layers.Select(x => x.startHeight).ToArray());
        material.SetFloatArray("baseBlends", layers.Select(x => x.blendStrength).ToArray());
        material.SetFloatArray("baseColorStrength", layers.Select(x => x.tintStrength).ToArray());

        UpdateMeshHeights(material, savedMinHeight, savedMaxHeight);
    }

    public void UpdateMeshHeights(Material material, float minHeight, float maxHeight)
    {
        savedMaxHeight = maxHeight;
        savedMinHeight = minHeight;

        material.SetFloat("minHeight", minHeight);
        material.SetFloat("maxHeight", maxHeight);
    }

    [System.Serializable]
    public class Layer
    {
        public Color tint;
        [Range(0, 1)]
        public float tintStrength;
        [Range(0, 1)]
        public float startHeight;
        [Range(0, 1)]
        public float blendStrength;
    }
}
