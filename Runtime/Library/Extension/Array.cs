using System;

namespace RealMethod
{
    public static class Array_Extension
    {
        public static bool IsValidIndex(this Array array, int index)
        {
            return index >= 0 && index < array.Length;
        }
        public static int FindIndex<T>(this Array array, T element)
        {
            return Array.IndexOf(array, element);
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
        /// <summary>
        /// Appends two arrays into a new combined array.
        /// </summary>
        /// <typeparam name="T">The element type of the arrays.</typeparam>
        /// <param name="first">The first array.</param>
        /// <param name="second">The second array.</param>
        /// <returns>A new array containing elements of both arrays.</returns>
        public static T[] CombineWith<T>(this T[] first, T[] second)
        {
            if (first == null) return second;
            if (second == null) return first;

            T[] result = new T[first.Length + second.Length];

            Array.Copy(first, 0, result, 0, first.Length);
            Array.Copy(second, 0, result, first.Length, second.Length);

            return result;
        }
    }
}