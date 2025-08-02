using UnityEngine;
using System.Collections.Generic;
using System.Collections;


namespace RealMethod
{
    public struct Hictionary<T> //: IEnumerable<KeyValuePair<Hash128, T>>
    {
        private Dictionary<Hash128, T> List;
        public int Count => List.Count;


        public Hictionary(int Prewarm)
        {
            List = new Dictionary<Hash128, T>(Prewarm);
        }

        // Access values
        public T this[string Name]
        {
            get => List[Hash128.Compute(Name)];
            set => List[Hash128.Compute(Name)] = value;
        }

        public bool IsValid()
        {
            return List != null;
        }
        public bool ContainsKey(string Name)
        {
            return List.ContainsKey(Hash128.Compute(Name));
        }
        public void Add(string Name, T Value)
        {
            List.Add(Hash128.Compute(Name), Value);
        }
        public bool Remove(string Name)
        {
            return List.Remove(Hash128.Compute(Name));
        }
        public bool TryGetValue(string Name, out T Result)
        {
            return List.TryGetValue(Hash128.Compute(Name), out Result);
        }
        public string[] GetKeys()
        {
            var keys = new List<Hash128>(List.Keys);
            string[] result = new string[keys.Count];
            for (int i = 0; i < keys.Count; i++)
            {
                result[i] = keys[i].ToString();
            }
            return result;
        }
        public T[] GetValues()
        {
            return new List<T>(List.Values).ToArray();
        }
        public void Clear()
        {
            List.Clear();
        }
        public string Find(T value)
        {
            foreach (var dictionary in List)
            {
                if (EqualityComparer<T>.Default.Equals(dictionary.Value, value))
                {
                    return dictionary.Key.ToString();
                }
            }
            return null;
        }
        public bool TryAdd(string Name, T Value)
        {
            return List.TryAdd(Hash128.Compute(Name), Value);
        }

        // // Implement IEnumerable<KeyValuePair<TKey, TValue>>
        // public IEnumerator<KeyValuePair<Hash128, T>> GetEnumerator()
        // {
        //     return List.GetEnumerator(); // Delegates to the internal dictionary
        // }

        // // Non-generic IEnumerable
        // IEnumerator IEnumerable.GetEnumerator()
        // {
        //     return GetEnumerator();
        // }
    }
}