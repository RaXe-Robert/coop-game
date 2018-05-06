using UnityEngine;

[CreateAssetMenu()]
public class ResourceMapSettings : UpdatableData
{
    public NoiseSettings NoiseSettings;

    [SerializeField]
    [Range(0,1)]
    private float densityThreshold;

    public float DensityThreshold { get { return 1f - densityThreshold; } }

#if UNITY_EDITOR

    protected override void OnValidate()
    {
        NoiseSettings.ValidateValues();

        densityThreshold = Mathf.Clamp(densityThreshold, 0f, 1f);

        base.OnValidate();
    }

#endif

}