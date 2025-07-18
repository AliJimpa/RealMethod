using System.Collections.Generic;

namespace RealMethod
{
    static class RM_Dictionary
    {
        public static void MergeDictionaries<TKey, TValue>(Dictionary<TKey, TValue> dict1, Dictionary<TKey, TValue> dict2)
        {
            // Add all elements from dict2 to dict1
            foreach (var kvp in dict2)
            {
                // Optionally, you can check if the key already exists in dict1
                if (!dict1.ContainsKey(kvp.Key))
                {
                    dict1.Add(kvp.Key, kvp.Value);
                }
                // If you want to overwrite the value in case the key already exists, use:
                // dict1[kvp.Key] = kvp.Value;
            }
        }
    }
}