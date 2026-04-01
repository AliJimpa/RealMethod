using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace RealMethod
{
    /// <summary>
    /// A dictionary keyed by string but internally hashes each key using <see cref="Hash128"/>.
    /// Provides stable lookups and near-zero collision risk.
    /// </summary>
    public class NameTable<T> : IEnumerable<KeyValuePair<Hash128, T>>
    {
        private Dictionary<Hash128, T> list;
        private static int GlobalIdCounter;
        private int InstanceId;


        /// <summary>
        /// Gets the number of entries.
        /// </summary>
        public int Count => list?.Count ?? 0;
        public bool IsValid => list != null;
        /// <summary>
        /// Gets all stored keys.
        /// </summary>
        public ICollection<string> Keys
        {
            get
            {
                List<string> keys = new();
                foreach (var k in list.Keys)
                    keys.Add(k.ToString());
                return keys;
            }
        }
        /// <summary>
        /// Gets all stored values.
        /// </summary>
        public IEnumerable<T> Values
        {
            get
            {
                return list.Values;
            }
        }
        public bool IsReadOnly => false;


        public NameTable(int prewarm = 10)
        {
            InstanceId = ++GlobalIdCounter;
            list = new Dictionary<Hash128, T>(prewarm);
        }


        // Indexer
        public T this[string name]
        {
            get => list[Key(name)];
            set => list[Key(name)] = value;
        }

        // Implement IEnumerable Interface
        public IEnumerator<KeyValuePair<Hash128, T>> GetEnumerator()
        {
            return list.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }


        // Functions
        public bool ContainsKey(string name)
        {
            return list.ContainsKey(Key(name));
        }
        public bool TryGetValue(string name, out T value)
        {
            return list.TryGetValue(Key(name), out value);
        }
        /// <summary>
        /// Adds or replaces an entry by name.
        /// </summary>
        public void Add(string name, T value)
        {
            list.Add(Key(name), value);
        }
        public bool TryAdd(string name, T value)
        {
            return list.TryAdd(Key(name), value);
        }
        /// <summary>
        /// Removes an entry by name.
        /// </summary>
        public bool Remove(string name)
        {
            return list.Remove(Key(name));
        }
        /// <summary>
        /// Removes all entries.
        /// </summary>
        public void Clear()
        {
            list.Clear();
        }
        public string[] GetKeys()
        {
            int c = list.Count;
            string[] result = new string[c];

            int i = 0;
            foreach (var kvp in list)
                result[i++] = kvp.Key.ToString();

            return result;
        }

        public string Find(T value)
        {
            foreach (var kvp in list)
            {
                if (EqualityComparer<T>.Default.Equals(kvp.Value, value))
                    return kvp.Key.ToString();
            }

            return null;
        }


        /// <summary>
        /// Computes a stable 128-bit key from a string.
        /// </summary>
        private Hash128 Key(string name) => Hash128.Compute(name);
    }
}