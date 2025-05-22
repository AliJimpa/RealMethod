using UnityEngine;
using System.Collections.Generic;
using NUnit.Framework;


namespace RealMethod
{
    public struct HashedKeyItem<T> where T : Object
    {
        public Dictionary<Hash128, T> List;

        public HashedKeyItem(int Prewarm)
        {
            List = new Dictionary<Hash128, T>(Prewarm);
        }

        public T this[string Name]
        {
            get => List[Hash128.Compute(Name)];
            set => List[Hash128.Compute(Name)] = value;
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
        public void Clear()
        {
            List.Clear();
        }


    }
}