using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RealMethod
{
    public static class Dictionary_Extension
    {
        public static T Wheel<T>(this Dictionary<T, float> DataBase)
        {
            if (DataBase == null || DataBase.Count == 0)
                throw new System.ArgumentException("Database is null or empty");

            // Ensure all weights are non-negative
            if (DataBase.Values.Any(w => w < 0))
                throw new System.ArgumentException("Weights cannot be negative");

            float totalWeight = DataBase.Values.Sum();

            if (totalWeight == 0)
                throw new System.InvalidOperationException("Sum of weights must be greater than zero");

            float rnd = Random.Range(0f, totalWeight);

            foreach (var item in DataBase)
            {
                rnd -= item.Value;
                if (rnd <= 0f)
                {
                    return item.Key;
                }
            }

            // Fallback (should rarely happen due to floating point errors)
            return DataBase.Keys.Last();
        }
        public static T Wheel<T>(this SerializableDictionary<T, float> DataBase)
        {
            if (DataBase == null || DataBase.Count == 0)
                throw new System.ArgumentException("Database is null or empty");

            // Ensure all weights are non-negative
            if (DataBase.Values.Any(w => w < 0))
                throw new System.ArgumentException("Weights cannot be negative");

            float totalWeight = DataBase.Values.Sum();

            if (totalWeight == 0)
                throw new System.InvalidOperationException("Sum of weights must be greater than zero");

            float rnd = Random.Range(0f, totalWeight);

            foreach (var item in DataBase)
            {
                rnd -= item.Value;
                if (rnd <= 0f)
                {
                    return item.Key;
                }
            }

            // Fallback (should rarely happen due to floating point errors)
            return DataBase.Keys.Last();
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