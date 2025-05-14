using System.Collections.Generic;

namespace RealMethod
{
    static class Random
    {
        public static T Shuffle<T>(T[] array)
        {
            if (array == null || array.Length == 0)
            {
                throw new System.ArgumentException("Array cannot be null or empty");
            }
            int randomIndex = UnityEngine.Random.Range(0, array.Length);
            return array[randomIndex];
        }

        public static T Shuffle<T>(List<T> list)
        {
            if (list == null || list.Count == 0)
            {
                throw new System.ArgumentException("List cannot be null or empty");
            }
            int randomIndex = UnityEngine.Random.Range(0, list.Count);
            return list[randomIndex];
        }

        public static T Wheel<T>(ref Dictionary<T, float> DataBase)
        {
            float[] weightlist = new float[DataBase.Values.Count];
            float Rnd = UnityEngine.Random.Range(0, Array.SumArray(weightlist));
            T Result = default(T);
            foreach (var item in DataBase)
            {
                Result = item.Key;
                Rnd -= item.Value;
                if (Rnd <= 0)
                {
                    break;
                }
            }
            return Result;
        }

    }
}