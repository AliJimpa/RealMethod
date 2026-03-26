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
    public struct SettingEnum
    {
        [SerializeField]
        private int Value; // <-- this name must match

        public SettingEnum(int index = 0)
        {
            Value = index;
        }
        public SettingEnum(byte index = 0)
        {
            Value = index;
        }


        public static implicit operator SettingEnum(int val)
        {
            return new SettingEnum(val);
        }
        public static implicit operator int(SettingEnum status)
        {
            return status.Value;
        }
        public static implicit operator SettingEnum(byte val)
        {
            return new SettingEnum(val);
        }
        public static implicit operator byte(SettingEnum status)
        {
            return (byte)status.Value;
        }
        public static bool operator ==(SettingEnum a, SettingEnum b)
        {
            return a.Value == b.Value;
        }
        public static bool operator !=(SettingEnum a, SettingEnum b)
        {
            return a.Value != b.Value;
        }
        public static bool operator <(SettingEnum a, SettingEnum b)
        {
            return a.Value < b.Value;
        }
        public static bool operator >(SettingEnum a, SettingEnum b)
        {
            return a.Value > b.Value;
        }
        public static bool operator <=(SettingEnum a, SettingEnum b)
        {
            return a.Value <= b.Value;
        }
        public static bool operator >=(SettingEnum a, SettingEnum b)
        {
            return a.Value >= b.Value;
        }


        public override string ToString()
        {
            return $"SettingEnum index [{Value}]";
        }
        public override bool Equals(object obj)
        {
            return obj is SettingEnum other && other.Value == Value;
        }
        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }


        // #if UNITY_EDITOR
        //         public SettingEnum(string IndexName)
        //         {
        //             byte result = 0;
        //             var ProjectSettings = AssetDatabase.LoadAssetAtPath<ProjectSettingAsset>("Assets/Resources/RealMethod/RealMethodSetting.asset");
        //             if (ProjectSettings)
        //             {
        //                 var names = ProjectSettings.Status;
        //                 if (names == null)
        //                     result = 0;
        //                 for (int i = 0; i < names.Count; i++)
        //                 {
        //                     if (names[i] == IndexName)
        //                     {
        //                         result = (byte)i;
        //                     }
        //                 }
        //             }
        //             else
        //             {
        //                 result = 0;
        //             }
        //             Value = result;
        //         }
        //         private string Name
        //         {
        //             get
        //             {
        //                 var ProjectSettings = AssetDatabase.LoadAssetAtPath<ProjectSettingAsset>("Assets/Resources/RealMethod/RealMethodSetting.asset");
        //                 if (ProjectSettings)
        //                 {
        //                     var names = ProjectSettings.Status;
        //                     if (names == null || Value >= names.Count)
        //                         return "Undefined";
        //                     return names[Value];
        //                 }
        //                 else
        //                 {
        //                     return "Undefined";
        //                 }
        //             }
        //         }
        //         public static implicit operator SettingEnum(string val)
        //         {
        //             return new SettingEnum(val);
        //         }
        //         public static implicit operator string(SettingEnum status)
        //         {
        //             var ProjectSettings = AssetDatabase.LoadAssetAtPath<ProjectSettingAsset>("Assets/Resources/RealMethod/RealMethodSetting.asset");
        //             if (ProjectSettings)
        //             {
        //                 var names = ProjectSettings.Status;
        //                 if (names == null || status.Value >= names.Count)
        //                     return "Undefined";
        //                 return names[status.Value];
        //             }
        //             else
        //             {
        //                 return "Undefined";
        //             }
        //         }
        // #endif



    }
}