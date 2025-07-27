using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RealMethod
{
    public static class Dictionary_Extension
    {
        public static T Wheel<T>(this Dictionary<T, float> DataBase)
        {
            float[] weightlist = DataBase.Values.ToArray();
            float Rnd = Random.Range(0, weightlist.Sum());
            T Result = default;
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
        public static T Wheel<T>(this SerializableDictionary<T, float> DataBase)
        {
            float[] weightlist = DataBase.Values.ToArray();
            float Rnd = Random.Range(0, weightlist.Sum());
            T Result = default;
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
        public static void MergeWith<TKey, TValue>(this Dictionary<TKey, TValue> dict1, Dictionary<TKey, TValue> dict2, bool overwrite = false)
        {
            // Add all elements from dict2 to dict1
            foreach (var kvp in dict2)
            {
                // Optionally, you can check if the key already exists in dict1
                if (!dict1.ContainsKey(kvp.Key))
                {
                    dict1.Add(kvp.Key, kvp.Value);
                }
                else
                {
                    if (overwrite)
                    {
                        dict1[kvp.Key] = kvp.Value;
                    }
                }
            }
        }
    }
}