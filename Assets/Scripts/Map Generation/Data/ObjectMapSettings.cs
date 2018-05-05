using UnityEngine;

[CreateAssetMenu()]
public class ObjectMapSettings : UpdatableData
{
    public NoiseSettings NoiseSettings;

    [SerializeField]
    [Range(0,1)]
    private float density;

    public float Density { get { return density * 2f - 1f; } }

#if UNITY_EDITOR

    protected override void OnValidate()
    {
        NoiseSettings.ValidateValues();

        density = Mathf.Clamp(density, 0f, 1f);

        base.OnValidate();
    }

#endif

}