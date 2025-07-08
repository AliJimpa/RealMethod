using System;

namespace RealMethod
{
    static class RM_Array
    {
        // Extention 
        public static bool IsValidIndex(this Array array, int index)
        {
            return index >= 0 && index < array.Length;
        }


        public static void Shuffle<T>(ref T[] array)
        {
            for (int i = array.Length - 1; i > 0; i--)
            {
                int j = UnityEngine.Random.Range(0, i + 1);
                T temp = array[i];
                array[i] = array[j];
                array[j] = temp;
            }
        }
    }
}