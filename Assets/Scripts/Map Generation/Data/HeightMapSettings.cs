﻿using System.Linq;
using UnityEngine;

[CreateAssetMenu()]
public class HeightMapSettings : UpdatableData
{
    public NoiseSettings NoiseSettings;
    
    public bool UseFalloff;
    public float HeightMultiplier;
    public AnimationCurve HeightCurve;

    public float MinHeight => HeightMultiplier * HeightCurve.Evaluate(0);
    public float MaxHeight => HeightMultiplier * HeightCurve.Evaluate(1);

    [SerializeField]
    private HeightMapLayer[] layers;
    public HeightMapLayer[] Layers => layers;

    public void ApplyToMaterial(Material material)
    {
        material.SetInt("layerCount", layers.Length);
        material.SetFloatArray("baseUseBiomeTint", layers.Select(x => (x.IsWater) ? 0f : 1f).ToArray()); // Convert to a float array since bool arrays aren't supported within shaders.
        material.SetColorArray("baseColors", layers.Select(x => x.Tint).ToArray());
        material.SetFloatArray("baseStartHeights", layers.Select(x => x.StartHeight).ToArray());
        material.SetFloatArray("baseBlends", layers.Select(x => x.BlendStrength).ToArray());
        material.SetFloatArray("baseColorStrength", layers.Select(x => x.TintStrength).ToArray());
    }

    public void UpdateMeshHeights(Material material, float minHeight, float maxHeight)
    {
        material.SetFloat("minHeight", minHeight);
        material.SetFloat("maxHeight", maxHeight);
    }
    
#if UNITY_EDITOR

    protected override void OnValidate()
    {
        NoiseSettings.ValidateValues();
        base.OnValidate();
    }

#endif
}

[System.Serializable]
public class HeightMapLayer
{
    [Tooltip("Water layers won't use the biome tint, also prevents objects from spawning.")]
    public bool IsWater;
    public Color Tint;
    [Range(0, 1)]
    public float TintStrength;
    [Range(0, 1)]
    public float StartHeight;
    [Range(0, 1)]
    public float BlendStrength;
}