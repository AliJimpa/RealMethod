using UnityEngine;

namespace RealMethod
{
    public static class Flag
    {
        private static readonly NameTable<bool> Flags = new NameTable<bool>();

        public static void Create(string name)
        {
            Flags.Add(name, false);
        }
        public static void Set(string name, bool DefaultValue)
        {
            if (Flags.ContainsKey(name))
            {
                Flags[name] = DefaultValue;
            }
            else
            {
                Debug.LogWarning($"Can't find Flag with {name}!");
            }
        }
        public static bool Get(string name)
        {
            if (Flags.ContainsKey(name))
            {
                return Flags[name];
            }
            else
            {
                Debug.LogWarning($"Can't find Flag with {name}!");
                return false;
            }
        }
        public static bool IsValid(string name)
        {
            return Flags.ContainsKey(name);
        }
        public static void Remove(string name)
        {
            if (Flags.ContainsKey(name))
            {
                Flags.Remove(name);
            }
            else
            {
                Debug.LogWarning($"Can't find Flag with {name}!");
            }
        }

    }
}