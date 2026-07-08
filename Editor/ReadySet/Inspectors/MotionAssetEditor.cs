using System;
using UnityEditor;
using UnityEngine;

namespace RealMethod.Editor
{
    [CustomEditor(typeof(MotionAsset), true)]
    public class TaskAssetCompWindow : UnityEditor.Editor
    {
        private MotionAsset BaseAsset;

        private void OnEnable()
        {
            BaseAsset = (MotionAsset)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            EditorGUILayout.LabelField("Debug.... ");
            if (BaseAsset != null)
            {
                EditorGUILayout.LabelField($"Status: {CheckStatus(BaseAsset)}");
                if (BaseAsset is IHandleBehaviourAction provider)
                {
                    if (provider.IsInfinit)
                    {
                        EditorGUILayout.LabelField($"Time: Infinit");
                    }
                    else
                    {
                        EditorGUILayout.LabelField($"Time: {Math.Round(provider.ElapsedTime, 2)}  ({Math.Round((1 - provider.NormalizedTime) * 100, 2)}%)");
                    }
                }

            }
        }


        private string CheckStatus(MotionAsset task)
        {
            return task.IsEnable ? "Enable" : "Disable";
        }
    }
}