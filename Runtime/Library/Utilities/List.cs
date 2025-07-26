using System.Collections.Generic;

namespace RealMethod
{
    public static class RM_List
    {
        public static bool IsValidIndex<T>(this List<T> list, int index)
        {
            return list != null && index >= 0 && index < list.Count;
        }
        public static void Shuffle<T>(this List<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = UnityEngine.Random.Range(0, i + 1);
                T temp = list[i];
                list[i] = list[j];
                list[j] = temp;
            }
        }
        public static T GetRandom<T>(this List<T> list)
        {
            int randomIndex = UnityEngine.Random.Range(0, list.Count);
            return list[randomIndex];
        }
        public static T GetRandomInRange<T>(this List<T> list, int minInclusive, int maxExclusive)
        {
            int randomIndex = UnityEngine.Random.Range(minInclusive, maxExclusive);
            return list[randomIndex];
        }
    }
}