using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace RealMethod
{
    [Serializable]
    public struct PropertyContainer
    {
        [SerializeField]
        private bool PublicProperty;
        [SerializeField]
        private bool ProtectedProperty;
        [SerializeField]
        private bool PrivateProperty;
        [SerializeField]
        private bool SerializeProperty; // For properties with [SerializeField]

        private PropertyInfo[] container;
        public bool isStore { get; private set; }

        public PropertyInfo this[int index]
        {
            get => container[index];
        }

        public void Scan(object target)
        {
            if (isStore)
            {
                Debug.LogError($"{this} PropertyContainer is full. Clean container first.");
                return;
            }

            if (target == null)
            {
                Debug.LogError($"{this} target object for scan is not valid");
                return;
            }

            List<PropertyInfo> result = new List<PropertyInfo>();
            Type type = target.GetType();
            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

            PropertyInfo[] properties = type.GetProperties(flags);

            foreach (PropertyInfo property in properties)
            {
                MethodInfo getter = property.GetGetMethod(true);
                if (getter == null) continue; // Skip properties without getter

                if (getter.IsPublic && PublicProperty)
                {
                    result.Add(property);
                    continue;
                }
                if (getter.IsFamily && ProtectedProperty)
                {
                    result.Add(property);
                    continue;
                }
                if (getter.IsPrivate && PrivateProperty)
                {
                    result.Add(property);
                    continue;
                }
                if (SerializeProperty && property.GetCustomAttribute<SerializeField>() != null)
                {
                    result.Add(property);
                    continue;
                }
            }

            container = result.ToArray();
            isStore = true;
        }

        public void Clean()
        {
            container = null;
            isStore = false;
        }

        public PropertyInfo[] GetProperties()
        {
            return container;
        }
    }
}
