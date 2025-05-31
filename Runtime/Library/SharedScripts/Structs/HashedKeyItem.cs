using UnityEngine;
using System.Collections.Generic;
using NUnit.Framework;


namespace RealMethod
{
    public struct HashedKeyItem<T>
    {
        private Dictionary<Hash128, T> List;
        public int Count => List.Count;

        public HashedKeyItem(int Prewarm)
        {
            List = new Dictionary<Hash128, T>(Prewarm);
        }

        public T this[string Name]
        {
            get => List[Hash128.Compute(Name)];
            set => List[Hash128.Compute(Name)] = value;
        }
        public bool IsValid(string Name)
        {
            return List.ContainsKey(Hash128.Compute(Name));
        }
        public bool IsValid(ItemAsset Asset)
        {
            return IsValid(Asset.Name);
        }
        public void AddItem(string Name, T Value)
        {
            List.Add(Hash128.Compute(Name), Value);
        }
        public bool RemoveItem(string Name)
        {
            return List.Remove(Hash128.Compute(Name));
        }
        public bool TryGetItem(string Name, out T Result)
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


    }
}