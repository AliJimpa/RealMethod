using System;
using UnityEngine;

namespace RealMethod
{
    /// <summary>
    /// This struct stores a game status as a byte index.
    /// The actual readable names are not stored here.
    /// Instead, the names are defined in a global list
    /// inside ProjectSettingsAsset that just compile in editor.
    /// </summary>
    [Serializable]
    public struct GlobalEnum
    {
        [SerializeField]
        private int Value; // <-- this name must match
        

        public static bool operator ==(GlobalEnum a, GlobalEnum b)
        {
            return a.Value == b.Value;
        }
        public static bool operator !=(GlobalEnum a, GlobalEnum b)
        {
            return a.Value != b.Value;
        }
        public static bool operator <(GlobalEnum a, GlobalEnum b)
        {
            return a.Value < b.Value;
        }
        public static bool operator >(GlobalEnum a, GlobalEnum b)
        {
            return a.Value > b.Value;
        }
        public static bool operator <=(GlobalEnum a, GlobalEnum b)
        {
            return a.Value <= b.Value;
        }
        public static bool operator >=(GlobalEnum a, GlobalEnum b)
        {
            return a.Value >= b.Value;
        }



        public override bool Equals(object obj)
        {
            return obj is GlobalEnum other && other.Value == Value;
        }
        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }


#if UNITY_EDITOR
        public override string ToString()
        {
            var ProjectSettings = RM_Framework.LoadProjectSetting_Editor();
            if (ProjectSettings)
            {
                var names = ProjectSettings.Status;
                RM_Framework.UnloadProjectSetting_Editor();
                if (names == null || Value >= names.Count)
                    return "Undefined";
                return names[Value];
            }
            else
            {
                return $"GlobalEnum index [{Value}]";
            }
        }
#else
        public override string ToString()
        {
            return $"GlobalEnum index [{Value}]";
        }
#endif



    }
}