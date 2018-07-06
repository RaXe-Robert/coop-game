using UnityEngine;

namespace Assets.Scripts.Map_Generation
{
    [System.Flags]
    public enum BiomeTypes
    {
        Forest = 0,
        Grassland = 1,
        Desert = 2,
    }

    [CreateAssetMenu(fileName = "New BiomeMapSettings", menuName = "Terrain Generation/BiomeMapSettings")]
    public class BiomeMapSettings : UpdatableData
    {
        public NoiseSettings NoiseSettings;

        public Biome[] Biomes;

#if UNITY_EDITOR

        protected override void OnValidate()
        {
            NoiseSettings.ValidateValues();
            base.OnValidate();
        }

#endif
    }

    [System.Serializable]
    public class Biome
    {
        [SerializeField]
        private BiomeTypes biomeType;

        [SerializeField]
        private Color color;

        [SerializeField]
        [Range(0, 1)]
        private float resourceDensity;

        public BiomeTypes BiomeType => biomeType;
        public string Name => biomeType.ToString();
        public Color Color => color;
        public float ResourceDensity => 1f - resourceDensity;
    }
}
