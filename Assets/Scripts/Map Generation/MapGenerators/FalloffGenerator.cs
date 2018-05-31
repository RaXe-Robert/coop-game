using UnityEngine;
using System.Collections;

namespace Assets.Scripts.Map_Generation
{
    public static class FalloffGenerator
    {
        public static float[,] GenerateFalloffMap(int size)
        {
            float[,] map = new float[size, size];

            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    float a = x / (float)size * 2 - 1;
                    float b = y / (float)size * 2 - 1;

                    float value = Mathf.Max(Mathf.Abs(a), Mathf.Abs(b));
                    map[x, y] = Evaluate(value);
                }
            }
            return map;
        }

        private static float Evaluate(float value)
        {
            float a = 3;
            float b = 2.2f;

            return Mathf.Pow(value, a) / (Mathf.Pow(value, a) + Mathf.Pow(b - b * value, a));
        }
    }
}
