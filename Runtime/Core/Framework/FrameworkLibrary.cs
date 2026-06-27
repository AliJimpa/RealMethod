using System;
using UnityEngine;

namespace RealMethod
{
    public static class RM_Framework
    {
        public const string BaseGameObjectName = "RealMethod";
        public const string ProjectSettingPath = "RealMethod/RealMethodSetting";
        public static bool IsCinemachineAvailable => Type.GetType("Cinemachine.CinemachineBrain, Cinemachine") != null;


        private static ProjectSettingAsset SettingAsset;
        public static ProjectSettingAsset LoadProjectSetting()
        {
            if (SettingAsset == null)
            {
                SettingAsset = Resources.Load<ProjectSettingAsset>(ProjectSettingPath);
            }
            return SettingAsset;
        }
        public static void UnloadProjectSetting()
        {
            if (SettingAsset != null)
            {
                Resources.UnloadAsset(SettingAsset);
            }
        }

#if UNITY_EDITOR
        public static ProjectSettingAsset LoadProjectSetting_Editor()
        {
            string path = $"Assets/Resources/{ProjectSettingPath}.asset";
            return UnityEditor.AssetDatabase.LoadAssetAtPath<ProjectSettingAsset>(path);
        }
        public static void UnloadProjectSetting_Editor()
        {
            //Noting
        }
#endif


    }
}