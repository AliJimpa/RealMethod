using System;

namespace RealMethod
{
    public static class Array_Extension
    {
        // Extention 
        public static bool IsValidIndex(this Array array, int index)
        {
            return index >= 0 && index < array.Length;
        }
        public static void Shuffle<T>(this T[] array)
        {
            for (int i = array.Length - 1; i > 0; i--)
            {
                int j = UnityEngine.Random.Range(0, i + 1);
                T temp = array[i];
                array[i] = array[j];
                array[j] = temp;
            }
        }
        public static T GetRandom<T>(this T[] array)
        {
            int randomIndex = UnityEngine.Random.Range(0, array.Length);
            return array[randomIndex];
        }
        public static T GetRandomInRange<T>(this T[] array, int minInclusive, int maxExclusive)
        {
            int randomIndex = UnityEngine.Random.Range(minInclusive, maxExclusive);
            return array[randomIndex];
        }
        public static float Sum(this float[] array)
        {
            float sum = 0f;
            foreach (float value in array)
            {
                sum += value;
            }
            return sum;
        }
        public static int Sum(this int[] array)
        {
            int sum = 0;
            foreach (int value in array)
            {
                sum += value;
            }
            return sum;
        }

    }
}