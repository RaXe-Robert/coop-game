using UnityEngine;

namespace Assets.Scripts.Map_Generation
{
    [CreateAssetMenu()]
    public class MeshSettings : UpdatableData
    {

        public const int MumOfSupportedLODs = 5;

        public const int NumOfSupportedChunkSizes = 9;
        public const int NumOfSupportedFlatshadedChunkSizes = 3;
        public static readonly int[] SupportedChunkSizes = { 48, 72, 96, 120, 144, 168, 192, 216, 240 };

        public const int NumOfSupportedChunkPartSizesRoot = 1;
        public static readonly int[] SupportedChunkPartSizesRoot = { 3 };

        public float MeshScale = 5f;
        public bool UseFlatShading;

        [Range(0, NumOfSupportedChunkSizes - 1)]
        public int ChunkSizeIndex;
        [Range(0, NumOfSupportedFlatshadedChunkSizes - 1)]
        public int FlatShadedChunkSizeIndex;

        [Range(0, NumOfSupportedChunkPartSizesRoot - 1)]
        public int ChunkPartSizeRootIndex = 0;

        // Number of vertices per line of a mesh rendered at LOD = 0. Includes the 2 extra vertices that are excluded from final mesh, but used for calculating normals
        public int NumVertsPerLine => SupportedChunkSizes[(UseFlatShading) ? FlatShadedChunkSizeIndex : ChunkSizeIndex] + 5;

        public float MeshWorldSize => (NumVertsPerLine - 3) * MeshScale;

        public int ChunkPartSizeRoot => SupportedChunkPartSizesRoot[ChunkPartSizeRootIndex];
    }
}
