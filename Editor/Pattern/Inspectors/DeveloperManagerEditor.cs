using UnityEditor;
using UnityEngine;

namespace RealMethod.Editor
{
    [CustomEditor(typeof(DeveloperManager), true)]
    public class DeveloperManagerEditor : UnityEditor.Editor
    {
        private DeveloperManager Comp;
        private void OnEnable()
        {
            Comp = (DeveloperManager)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Open"))
            {
                Comp.Open();
            }
            if (GUILayout.Button("Close"))
            {
                Comp.Close();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.LabelField($"Status: {IsOpen()}");
        }


        private string IsOpen()
        {
            return Comp.IsOpen ? "Open" : "Close";
        }
    }
}