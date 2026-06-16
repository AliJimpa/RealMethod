using System;
using UnityEngine;

namespace RealMethod
{
    public static class RM_Framework
    {
        public const string BaseGameObjectName = "RealMethod";
        public const string ProjectSettingPath = "RealMethod/RealMethodSetting";
        public static ProjectSettingAsset LoadProjectSetting()
        {
            return Resources.Load<ProjectSettingAsset>(ProjectSettingPath);
        }
#if UNITY_EDITOR
        public static string ProjectSettingPath_Editor = "Assets/Resources/RealMethod/RealMethodSetting.asset";
        public static ProjectSettingAsset LoadProjectSetting_Editor()
        {
            return UnityEditor.AssetDatabase.LoadAssetAtPath<ProjectSettingAsset>(ProjectSettingPath_Editor);
        }
#endif

        public static bool IsCinemachineAvailable => Type.GetType("Cinemachine.CinemachineBrain, Cinemachine") != null;
    }
}