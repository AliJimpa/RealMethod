using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

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

        public GlobalEnum(int index = 0)
        {
            Value = index;
        }
        public GlobalEnum(byte index = 0)
        {
            Value = index;
        }


        public static implicit operator GlobalEnum(int val)
        {
            return new GlobalEnum(val);
        }
        public static implicit operator int(GlobalEnum status)
        {
            return status.Value;
        }
        public static implicit operator GlobalEnum(byte val)
        {
            return new GlobalEnum(val);
        }
        public static implicit operator byte(GlobalEnum status)
        {
            return (byte)status.Value;
        }
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
            var ProjectSettings = AssetDatabase.LoadAssetAtPath<ProjectSettingAsset>("Assets/Resources/RealMethod/RealMethodSetting.asset");
            if (ProjectSettings)
            {
                var names = ProjectSettings.Status;
                if (names == null || Value >= names.Count)
                    return "Undefined";
                return names[Value];
            }
            else
            {
                return $"SettingEnum index [{Value}]";
            }

        }
#else
        public override string ToString()
        {
            return $"SettingEnum index [{Value}]";
        }
#endif



    }
}