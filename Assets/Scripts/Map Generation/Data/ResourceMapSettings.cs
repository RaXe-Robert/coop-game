using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Map_Generation
{
    [CreateAssetMenu()]
    public class ResourceMapSettings : UpdatableData
    {
        [SerializeField]
        private NoiseSettings noiseSettings;

        [SerializeField]
        private WorldResourceEntry[] worldResourceEntries;

        public NoiseSettings NoiseSettings => noiseSettings;
        public WorldResourceEntry[] WorldResourceEntries => worldResourceEntries;

#if UNITY_EDITOR

        protected override void OnValidate()
        {
            NoiseSettings.ValidateValues();

            base.OnValidate();
        }

#endif
    }

    [System.Serializable]
    public class WorldResourceEntry
    {
        [SerializeField]
        private GameObject worldResourcePrefab;
        public GameObject WorldResourcePrefab => worldResourcePrefab;
        
        [SerializeField]
        [EnumFlags]
        private BiomeTypes biomes;

        public List<int> GetBiomes()
        {
            List<int> selectedElements = new List<int>();
            for (int i = 0; i < System.Enum.GetValues(typeof(BiomeTypes)).Length; i++)
            {
                int layer = 1 << i;
                if (((int) biomes & layer) != 0)
                {
                    selectedElements.Add(i);
                }
            }

            return selectedElements;
        }
    }
}