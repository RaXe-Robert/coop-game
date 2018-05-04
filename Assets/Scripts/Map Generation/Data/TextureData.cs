using System.Linq;
using UnityEngine;

[CreateAssetMenu()]
public class TextureData : UpdatableData
{
    [SerializeField]
    private Layer[] layers;
    
    public void ApplyToMaterial(Material material)
    {
        material.SetInt("layerCount", layers.Length);
        material.SetFloatArray("baseUseBiomeTint", layers.Select(x => x.UseBiomeTint).ToArray());
        material.SetColorArray("baseColors", layers.Select(x => x.tint).ToArray());
        material.SetFloatArray("baseStartHeights", layers.Select(x => x.startHeight).ToArray());
        material.SetFloatArray("baseBlends", layers.Select(x => x.blendStrength).ToArray());
        material.SetFloatArray("baseColorStrength", layers.Select(x => x.tintStrength).ToArray());
    }

    public void UpdateMeshHeights(Material material, float minHeight, float maxHeight)
    {
        material.SetFloat("minHeight", minHeight);
        material.SetFloat("maxHeight", maxHeight);
    }
    
    [System.Serializable]
    public class Layer
    {
        [Range(0, 1)]
        public float UseBiomeTint;
        public Color tint;
        [Range(0, 1)]
        public float tintStrength;
        [Range(0, 1)]
        public float startHeight;
        [Range(0, 1)]
        public float blendStrength;
    }

#if UNITY_EDITOR

    protected override void OnValidate()
    {
        float[] layersUseBiomeTint = layers.Select(x => x.UseBiomeTint).ToArray();
        for (int i = 0; i < layersUseBiomeTint.Length; i++)
            layers[i].UseBiomeTint = layersUseBiomeTint[i] < 0.5f ? 0f : 1f;

        base.OnValidate();
    }

#endif
}
