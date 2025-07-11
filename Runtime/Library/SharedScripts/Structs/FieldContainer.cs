using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace RealMethod
{
    [Serializable ]
    public struct FieldContainer
    {
        [SerializeField]
        private bool PublicField;
        [SerializeField]
        private bool ProtectedField;
        [SerializeField]
        private bool PrivateField;
        [SerializeField]
        private bool SerializeField;

        private FieldInfo[] container;
        public bool isStore { get; private set; }


        public FieldInfo this[int index]
        {
            get => container[index];
        }

        public void Scan(object target)
        {
            if (isStore)
            {
                Debug.LogError($"{this} First Clean Storage");
            }

            List<FieldInfo> Result = new List<FieldInfo>();
            Type type = target.GetType();
            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

            FieldInfo[] fields = type.GetFields(flags);

            foreach (FieldInfo field in fields)
            {
                if (field.IsPublic)
                {
                    if (PublicField)
                    {
                        Result.Add(field);
                        continue;
                    }
                }
                if (field.IsFamily)
                {
                    if (ProtectedField)
                    {
                        Result.Add(field);
                        continue;
                    }
                }
                if (field.IsPrivate)
                {
                    if (PrivateField)
                    {
                        Result.Add(field);
                        continue;
                    }
                }
                if (SerializeField)
                {
                    if (field.GetCustomAttribute<SerializeField>() != null)
                    {
                        Result.Add(field);
                        continue;
                    }
                }
            }

            container = Result.ToArray();
            isStore = true;
        }
        public void Clean()
        {
            container = null;
            isStore = false;
        }
        public FieldInfo[] GetFields()
        {
            return container;
        }

    }
}