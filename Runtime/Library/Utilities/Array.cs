namespace RealMethod
{
    static class Array
    {
        public static bool IsValidIndex<T>(T[] array, int index)
        {
            return index >= 0 && index < array.Length;
        }

        public static float SumArray(float[] array)
        {
            float sum = 0f;
            foreach (float value in array)
            {
                sum += value;
            }
            return sum;
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